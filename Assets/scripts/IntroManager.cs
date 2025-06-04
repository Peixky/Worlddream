using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("UI Panel 名稱設定")]
    public string introPanelName = "IntroPanel";
    public string startTextPanelName = "StartTextPanel";
    public string gameOverPanelName = "GameOverPanel";

    [Header("遊戲結束 UI 元素名稱設定 (在場景中查找)")]
    public string loserTextUIName = "LoserText";

    private GameObject currentIntroPanel;
    private GameObject currentStartTextPanel;
    private GameObject currentGameOverPanel;

    private TextMeshProUGUI loserTextUI;

    [Header("時間設定")]
    public float vsScreenDuration = 3f;
    public float startTextDuration = 3f;
    public float gameOverFadeDelay = 1.0f;

    public enum GameState
    {
        Intro,
        Starting,
        Playing,
        Paused,
        GameOver
    }

    public static GameState currentGameState = GameState.Intro;

    void Awake()
    {
        if (FindObjectsByType<IntroManager>(FindObjectsSortMode.None).Length > 1) 
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializeCurrentSceneUI();
        
        if (SceneManager.GetActiveScene().buildIndex == 0) 
        {
            Time.timeScale = 1f;
            currentGameState = GameState.Paused;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeCurrentSceneUI();

        if (GameProgressionManager.instance != null && scene.name == GameProgressionManager.instance.gameScenes[2]) 
        {
            Debug.Log("步驟 1: 顯示 V.S. 圖片畫面。遊戲暫停中。");
            Time.timeScale = 0f;
            currentGameState = GameState.Intro;
            StartCoroutine(IntroRoutine());
        }
        else 
        {
            Debug.Log($"IntroManager: 載入 Scene '{scene.name}'。遊戲進入 Playing 狀態。");
            Time.timeScale = 1f;
            currentGameState = GameState.Playing;
            if (currentIntroPanel != null) currentIntroPanel.SetActive(false);
            if (currentStartTextPanel != null) currentStartTextPanel.SetActive(false);
        }

        if (currentGameOverPanel != null) currentGameOverPanel.SetActive(false);
        if (loserTextUI != null) loserTextUI.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void InitializeCurrentSceneUI()
    {
        Canvas currentCanvas = FindFirstObjectByType<Canvas>(); 

        if (currentCanvas != null)
        {
            Transform introPanelTransform = currentCanvas.transform.Find(introPanelName);
            currentIntroPanel = introPanelTransform != null ? introPanelTransform.gameObject : null;

            Transform startTextPanelTransform = currentCanvas.transform.Find(startTextPanelName);
            currentStartTextPanel = startTextPanelTransform != null ? startTextPanelTransform.gameObject : null;

            Transform gameOverPanelTransform = currentCanvas.transform.Find(gameOverPanelName);
            currentGameOverPanel = gameOverPanelTransform != null ? gameOverPanelTransform.gameObject : null;

            GameObject loserTextObj = GameObject.Find(loserTextUIName);
            if (loserTextObj != null) loserTextUI = loserTextObj.GetComponent<TextMeshProUGUI>();
            else loserTextUI = null;

            if (currentIntroPanel == null) Debug.LogWarning($"IntroManager: 未在 Canvas 下找到名為 '{introPanelName}' 的 Panel！", currentCanvas);
            if (currentStartTextPanel == null) Debug.LogWarning($"IntroManager: 未在 Canvas 下找到名為 '{startTextPanelName}' 的 Panel！", currentCanvas);
            if (currentGameOverPanel == null) Debug.LogWarning($"IntroManager: 未在 Canvas 下找到名為 '{gameOverPanelName}' 的 Panel！", currentCanvas);
            if (loserTextUI == null) Debug.LogWarning($"IntroManager: 未在場景中找到名為 '{loserTextUIName}' 的 TextMeshProUGUI 元件！", this);
        }
        else
        {
            Debug.LogWarning("IntroManager: 當前 Scene 未找到 Canvas！", this);
            currentIntroPanel = null; currentStartTextPanel = null; currentGameOverPanel = null; loserTextUI = null;
        }

        if (currentIntroPanel != null) currentIntroPanel.SetActive(false);
        if (currentStartTextPanel != null) currentStartTextPanel.SetActive(false);
        if (currentGameOverPanel != null) currentGameOverPanel.SetActive(false);
        if (loserTextUI != null) loserTextUI.gameObject.SetActive(false);
    }

    IEnumerator IntroRoutine()
    {
        if (currentIntroPanel == null || currentStartTextPanel == null)
        {
            InitializeCurrentSceneUI();
            if (currentIntroPanel == null && currentStartTextPanel == null) yield break;
        }

        if (currentIntroPanel != null) currentIntroPanel.SetActive(true);
        else Debug.LogError($"IntroManager: 無法顯示 Intro Panel '{introPanelName}'，請檢查名稱及是否存在於當前 Scene。", this);

        Debug.Log("步驟 1: 顯示 V.S. 圖片畫面。遊戲暫停中。");
        yield return new WaitForSecondsRealtime(vsScreenDuration);

        if (currentIntroPanel != null) currentIntroPanel.SetActive(false);

        if (currentStartTextPanel != null) currentStartTextPanel.SetActive(true);
        else Debug.LogError($"IntroManager: 無法顯示 Start Text Panel '{startTextPanelName}'，請檢查名稱及是否存在於當前 Scene。", this);

        TextMeshProUGUI startTextMesh = currentStartTextPanel?.GetComponentInChildren<TextMeshProUGUI>(true);
        if (startTextMesh != null)
        {
            startTextMesh.text = "START"; 
            if (!startTextMesh.gameObject.activeSelf) startTextMesh.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"IntroManager: 未找到 '{startTextPanelName}' 下的 TextMeshProUGUI 元件！", currentStartTextPanel);
        }

        Debug.Log("步驟 2: V.S. 圖片消失延遲結束。顯示 START 文字。");
        yield return new WaitForSecondsRealtime(startTextDuration);

        if (currentStartTextPanel != null) currentStartTextPanel.SetActive(false);
        Debug.Log("步驟 3: START 文字消失。遊戲開始！");

        Time.timeScale = 1f;
        currentGameState = GameState.Playing;

        GameEvents.OnGameStart?.Invoke();
    }

    public static void ShowGameOver()
    {
        if (currentGameState != GameState.GameOver)
        {
            Time.timeScale = 0f;
            currentGameState = GameState.GameOver;

            IntroManager instance = FindFirstObjectByType<IntroManager>();
            if (instance != null)
            {
                if (instance.currentGameOverPanel != null)
                {
                    instance.currentGameOverPanel.SetActive(true);
                    Debug.Log("IntroManager: 顯示遊戲結束畫面 (透過 GameOverPanel)。");
                }
                else if (instance.loserTextUI != null)
                {
                    Debug.LogWarning("IntroManager: GameOverPanel 未找到，但嘗試顯示 LoserText。", instance);
                }
                else
                {
                    Debug.LogError($"IntroManager: 無法顯示遊戲結束畫面！請確保當前 Scene 中有'{instance?.gameOverPanelName}' Panel 或 '{instance?.loserTextUIName}' UI 元件。", instance);
                    return;
                }
                
                instance.StartCoroutine(instance.ShowLoserTextFlow());
            }
            else
            {
                Debug.LogError("IntroManager: 無法找到 IntroManager 實例來顯示遊戲結束畫面。", null);
            }
        }
    }

    IEnumerator ShowLoserTextFlow()
    {
        FadeEffect fadeEffect = null;
        if (currentGameOverPanel != null)
        {
            fadeEffect = currentGameOverPanel.GetComponentInChildren<FadeEffect>(true);
        }

        if (fadeEffect != null)
        {
            fadeEffect.StartFadeIn(); 
            yield return new WaitForSecondsRealtime(fadeEffect.fadeDuration); 
        }
        else
        {
            Debug.LogWarning("IntroManager: 未找到 GameOverPanel 下的 FadeEffect 腳本或 GameOverPanel 不存在！畫面不會漸變。", currentGameOverPanel);
            yield return new WaitForSecondsRealtime(gameOverFadeDelay); 
        }

        if (loserTextUI != null)
        {
            loserTextUI.gameObject.SetActive(true); 
            Debug.Log("IntroManager: LoserText 已顯示。");
        }
        else
        {
            Debug.LogWarning("IntroManager: LoserText UI 未找到！無法顯示遊戲結束文字。", this);
        }
    }

    public static void PauseGame()
    {
        if (currentGameState == GameState.Playing)
        {
            Time.timeScale = 0f;
            currentGameState = GameState.Paused;
            Debug.Log("遊戲暫停。");
        }
    }

    public static void ResumeGame()
    {
        if (currentGameState == GameState.Paused)
        {
            Time.timeScale = 1f;
            currentGameState = GameState.Playing;
            Debug.Log("遊戲恢復。");
        }
    }
}