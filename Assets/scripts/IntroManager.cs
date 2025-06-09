using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.Collections;
using System; 
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))] 
public class IntroManager : MonoBehaviour
{
    [Header("UI 物件拖曳區（請從 Inspector 拖進來）")]
    public GameObject introPanel;
    public GameObject startTextPanel;
    public GameObject fadePanel;        
    public TextMeshProUGUI loserTextUI; 

    [Header("時間設定")]
    public float vsScreenDuration = 3f;
    public float startTextDuration = 3f;
    public float gameOverFadeDelay = 1.0f; 

    [Header("音效設定")]
    public AudioClip gameStartSoundEffect;

    public static GameProgressionManager.GameState currentGameState = GameProgressionManager.GameState.Intro;

    private AudioSource audioSource; 
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("IntroManager: AudioSource 組件遺失！無法播放音效。請將 AudioSource 添加到此 GameObject。", this);
            enabled = false; 
            return;
        }
    }

    void Start()
    {
        Debug.Log($"IntroManager: 在 '{SceneManager.GetActiveScene().name}' 啟動。");

        
        if (introPanel != null) introPanel.SetActive(false);
        if (startTextPanel != null) startTextPanel.SetActive(false);
        if (fadePanel != null) fadePanel.SetActive(false); 
        if (loserTextUI != null) loserTextUI.gameObject.SetActive(false); 

        
        Time.timeScale = 0f;
        GameProgressionManager.currentGameState = GameProgressionManager.GameState.Intro;

        // 啟動 Intro 流程
        StartCoroutine(IntroRoutine());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 訂閱玩家死亡事件
        GameEvents.OnPlayerDied += OnPlayerDeathEvent; 
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // 取消訂閱玩家死亡事件，防止記憶體洩漏
        GameEvents.OnPlayerDied -= OnPlayerDeathEvent; 
    }
    
    // 處理玩家死亡事件的方法
    private void OnPlayerDeathEvent()
    {
        Debug.Log("IntroManager: 接收到玩家死亡事件，觸發遊戲結束流程！");
        ShowGameOver(); // 呼叫現有的顯示遊戲結束方法
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (introPanel != null) introPanel.SetActive(false);
        if (startTextPanel != null) startTextPanel.SetActive(false);
        if (fadePanel != null) fadePanel.SetActive(false);
        if (loserTextUI != null) loserTextUI.gameObject.SetActive(false);

        if (GameProgressionManager.instance != null && scene.name == GameProgressionManager.instance.gameScenes[2])
        {
            Debug.Log("步驟 1: 顯示 V.S. 圖片畫面。遊戲暫停中。");
            Time.timeScale = 0f;
            currentGameState = GameProgressionManager.GameState.Intro;
            
        }
        else
        {
            Debug.Log($"IntroManager: 載入 Scene '{scene.name}' (非 Level3)。遊戲進入 Playing 狀態。");
            Time.timeScale = 1f;
            currentGameState = GameProgressionManager.GameState.Playing;
            if (introPanel != null) introPanel.SetActive(false);
            if (startTextPanel != null) startTextPanel.SetActive(false);
        }
    }

    IEnumerator IntroRoutine()
    {
        // 顯示 V.S. 畫面
        if (introPanel != null) introPanel.SetActive(true);
        Debug.Log("步驟 1: 顯示 V.S. 圖片畫面。遊戲暫停中。");

        yield return new WaitForSecondsRealtime(vsScreenDuration);

        // 隱藏 V.S. 畫面，顯示 START 文字
        if (introPanel != null) introPanel.SetActive(false);
        if (startTextPanel != null) startTextPanel.SetActive(true);

        var startText = startTextPanel.GetComponentInChildren<TextMeshProUGUI>(true);
        if (startText != null)
        {
            startText.text = "GAME START!"; // 設定為 GAME START!
            startText.gameObject.SetActive(true);
        }

        // 播放 Game Start 音效
        if (gameStartSoundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameStartSoundEffect);
            Debug.Log("IntroManager: 播放 Game Start 音效。");
        }
        else if (gameStartSoundEffect == null)
        {
            Debug.LogWarning("IntroManager: Game Start 音效 AudioClip 未設定！");
        }

        Debug.Log("步驟 2: 顯示 GAME START! 文字。");

        yield return new WaitForSecondsRealtime(startTextDuration);

        // 隱藏 START 文字，遊戲開始
        if (startTextPanel != null) startTextPanel.SetActive(false);
        Debug.Log("步驟 3: START 文字消失。遊戲開始！");

        Time.timeScale = 1f;
        GameProgressionManager.currentGameState = GameProgressionManager.GameState.Playing;
        GameEvents.OnGameStart?.Invoke(); // 觸發遊戲開始事件
    }

    public static void ShowGameOver()
    {
        var instance = FindFirstObjectByType<IntroManager>();
        if (instance == null)
        {
            Debug.Log($"IntroManager: 未在當前場景 ({SceneManager.GetActiveScene().name}) 找到 IntroManager 實例，不顯示 GameOver UI。");
            return;
        }

        if (GameProgressionManager.instance != null && SceneManager.GetActiveScene().name == GameProgressionManager.instance.gameScenes[2])
        {
            if (GameProgressionManager.currentGameState != GameProgressionManager.GameState.GameOver)
            {
                Time.timeScale = 0f; // 暫停遊戲時間
                GameProgressionManager.currentGameState = GameProgressionManager.GameState.GameOver; // 設定遊戲狀態

                // 顯示 FadePanel
                if (instance.fadePanel != null)
                {
                    instance.fadePanel.SetActive(true);
                    Debug.Log("IntroManager: 顯示遊戲結束畫面 (透過 FadePanel)。");
                }
                else
                {
                    Debug.LogWarning("IntroManager: FadePanel 未設定或未拖曳！");
                }

                instance.StartCoroutine(instance.ShowLoserTextFlow());
            }
        }
        else
        {
            // 如果不在 Level3GameScene，且 IntroManager 實例意外存在，則不做 UI 顯示
            Debug.Log($"IntroManager: 玩家在非 Level3GameScene 死亡 ({SceneManager.GetActiveScene().name})。遊戲結束畫面將不顯示。");
            Time.timeScale = 0f; // 遊戲時間依然暫停
            GameProgressionManager.currentGameState = GameProgressionManager.GameState.GameOver;
        }
    }

    IEnumerator ShowLoserTextFlow()
    {
        FadeEffect fadeEffect = null;
        if (fadePanel != null)
        {
            fadeEffect = fadePanel.GetComponentInChildren<FadeEffect>(true);
            if (fadeEffect == null)
            {
                Debug.LogWarning("IntroManager: FadePanel 上或其子物件中未找到 FadeEffect 組件！");
            }
        }
        
        // 執行漸變效果
        if (fadeEffect != null)
        {
            fadeEffect.StartFadeIn(); 
            yield return new WaitForSecondsRealtime(fadeEffect.fadeDuration);
        }
        else
        {
            Debug.LogWarning("IntroManager: FadeEffect 沒有設定或不存在！畫面不會漸變。直接延遲。");
            yield return new WaitForSecondsRealtime(gameOverFadeDelay);
        }

        // 顯示 LoserText
        if (loserTextUI != null)
        {
            loserTextUI.gameObject.SetActive(true);
            loserTextUI.text = "GAME OVER"; 
            Debug.Log("IntroManager: 顯示 LoserText！");
        }
        else
        {
            Debug.LogWarning("IntroManager: LoserTextUI 未設定！無法顯示遊戲結束文字。");
        }

        yield return new WaitForSecondsRealtime(2.0f); // 稍微增加顯示時間，以便玩家看清

        Debug.Log("IntroManager: 玩家死亡流程結束，載入玩家死亡劇情 Scene。");
        GameProgressionManager.AdvanceStory(); 
        GameProgressionManager.LoadNextStoryScene();
    }

    
    public static void PauseGame()
    {
        if (GameProgressionManager.currentGameState == GameProgressionManager.GameState.Playing)
        {
            Time.timeScale = 0f;
            GameProgressionManager.currentGameState = GameProgressionManager.GameState.Paused;
            Debug.Log("遊戲暫停。");
        }
    }

    public static void ResumeGame()
    {
        if (GameProgressionManager.currentGameState == GameProgressionManager.GameState.Paused)
        {
            Time.timeScale = 1f;
            GameProgressionManager.currentGameState = GameProgressionManager.GameState.Playing;
            Debug.Log("遊戲恢復。");
        }
    }
}