using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System; // Required for Action events, though not directly used in this snippet's public API
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))] // Ensures AudioSource component is present on this GameObject
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

    // currentGameState 現在是 GameProgressionManager 管理的全局狀態
    public static GameProgressionManager.GameState currentGameState = GameProgressionManager.GameState.Intro;

    private AudioSource audioSource; // 私有變數來儲存 AudioSource 組件

    void Awake()
    {
        // 在 Awake 階段獲取 AudioSource 組件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("IntroManager: AudioSource 組件遺失！無法播放音效。請將 AudioSource 添加到此 GameObject。", this);
            enabled = false; // 如果沒有 AudioSource，禁用腳本
            return;
        }
    }

    void Start()
    {
        Debug.Log($"IntroManager: 在 '{SceneManager.GetActiveScene().name}' 啟動。");

        // 初始化時確保所有相關 UI 都是隱藏的
        if (introPanel != null) introPanel.SetActive(false);
        if (startTextPanel != null) startTextPanel.SetActive(false);
        if (fadePanel != null) fadePanel.SetActive(false);
        if (loserTextUI != null) loserTextUI.gameObject.SetActive(false);

        // 設定遊戲狀態並暫停時間以顯示 V.S. 畫面
        Time.timeScale = 0f;
        GameProgressionManager.currentGameState = GameProgressionManager.GameState.Intro;

        // 啟動 Intro 流程
        StartCoroutine(IntroRoutine());
    }

    // 這個方法現在變得多餘，因為 IntroManager 只存在於 Level3GameScene，
    // 其初始化邏輯已移至 Start()。保留此方法是為了避免對現有專案結構造成額外變動。
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 這些邏輯已在 Start() 中處理，因為 IntroManager 只存在於 Level3GameScene
        // 確保UI初始化
        if (introPanel != null) introPanel.SetActive(false);
        if (startTextPanel != null) startTextPanel.SetActive(false);
        if (fadePanel != null) fadePanel.SetActive(false);
        if (loserTextUI != null) loserTextUI.gameObject.SetActive(false);

        // 如果場景不是 Level3GameScene，但這個 IntroManager 被錯誤地保留下來，則直接進入 Playing 狀態
        // 否則，按照 Level3GameScene 的流程啟動
        if (GameProgressionManager.instance != null && scene.name == GameProgressionManager.instance.gameScenes[2])
        {
            Debug.Log("步驟 1: 顯示 V.S. 圖片畫面。遊戲暫停中。");
            Time.timeScale = 0f;
            currentGameState = GameProgressionManager.GameState.Intro;
            // StartCoroutine(IntroRoutine()); // 已在 Start() 中呼叫
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

    // OnEnable 和 OnDisable 需要重新考慮，因為 IntroManager 不再是 DontDestroyOnLoad。
    // 如果它只在 Level3GameScene 存在，則這些訂閱和取消訂閱的時機可能需要根據生命週期調整。
    // 但為保持與你提供程式碼結構的一致性，暫時保持原樣。
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
            // 如果在 Level3GameScene 以外的場景被呼叫，IntroManager 實例將為 null，則直接返回
            Debug.Log($"IntroManager: 未在當前場景 ({SceneManager.GetActiveScene().name}) 找到 IntroManager 實例，不顯示 GameOver UI。");
            return;
        }

        // 確保只在 Level3GameScene 中生效
        if (GameProgressionManager.instance != null && SceneManager.GetActiveScene().name == GameProgressionManager.instance.gameScenes[2])
        {
            if (GameProgressionManager.currentGameState != GameProgressionManager.GameState.GameOver)
            {
                Time.timeScale = 0f;
                GameProgressionManager.currentGameState = GameProgressionManager.GameState.GameOver;

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
        }

        // 執行漸變效果
        if (fadeEffect != null)
        {
            fadeEffect.StartFadeIn();
            yield return new WaitForSecondsRealtime(fadeEffect.fadeDuration);
        }
        else
        {
            Debug.LogWarning("IntroManager: FadeEffect 沒有設定或不存在！畫面不會漸變。");
            yield return new WaitForSecondsRealtime(gameOverFadeDelay);
        }

        // 顯示 LoserText
        if (loserTextUI != null)
        {
            loserTextUI.gameObject.SetActive(true);
            Debug.Log("IntroManager: 顯示 LoserText！");
        }
        else
        {
            Debug.LogWarning("IntroManager: LoserTextUI 未設定！無法顯示遊戲結束文字。");
        }

        // 等待一小段時間讓玩家看清文字
        yield return new WaitForSecondsRealtime(1.0f);

        Debug.Log("IntroManager: 玩家死亡流程結束，載入玩家死亡劇情 Scene。");
        GameProgressionManager.LoadNextStoryScene(); // 載入玩家死亡劇情 Scene
    }

    // PauseGame 和 ResumeGame 方法已移至 GameProgressionManager
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