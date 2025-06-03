using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果使用 TextMeshPro 文本
using System.Collections; // 如果使用協程
using System; // 使用 Action 事件
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    // UI Panel 名稱設定 (在 Inspector 中設定，讓腳本在每次 Scene 載入時自動查找)
    [Header("UI Panel 名稱設定")]
    public string introPanelName = "IntroPanel"; // V.S. 畫面 Panel 的名稱
    public string startTextPanelName = "StartTextPanel"; // "START" 文字 Panel 的名稱
    public string gameOverPanelName = "GameOverPanel"; // 遊戲結束 Panel 的名稱

    // 實際的 Panel 引用 (在運行時獲取)
    private GameObject currentIntroPanel;
    private GameObject currentStartTextPanel;
    private GameObject currentGameOverPanel;

    // 遊戲結束 UI 元素 (在 GameOverPanel 下)
    [Header("遊戲結束 UI 元素 (在 GameOverPanel 下)")]
    public TextMeshProUGUI loserTextUI; // 拖曳 LoserText 物件上的 TextMeshProUGUI 元件到這裡
    public Button restartButton; // 拖曳「再玩一次」按鈕的 Button 元件到這裡

    // 時間設定
    [Header("時間設定")]
    public float vsScreenDuration = 3f; // V.S. 畫面停留時間 (秒)
    public float startTextDuration = 3f; // "START" 文字停留時間 (秒)
    public float gameOverFadeDelay = 1.0f; // 遊戲結束畫面漸變到文字顯示的延遲 (畫面變暗的時間)

    // 遊戲狀態的枚舉 (Enum)
    public enum GameState
    {
        Intro,      // 遊戲介紹中 (V.S. 圖片顯示，時間暫停)
        Starting,   // 準備開始 ("START" 文字顯示，時間暫停)
        Playing,    // 遊戲正在進行中 (時間流動)
        Paused,     // 遊戲暫停 (時間暫停)
        GameOver    // 遊戲結束 (時間暫停)
    }

    public static GameState currentGameState = GameState.Intro; // 靜態變數，初始為 Intro

    void Awake()
    {
        // 單例模式：確保這個腳本在遊戲中只有一個實例
        // 對於 Unity 2021.1 或更早版本，請使用 FindObjectsOfType
        // 這裡修改為查找所有類型為 IntroManager 的物件，包括非激活的
        // 如果數量大於 1，說明有重複的，就銷毀自己
        if (FindObjectsOfType<IntroManager>(true).Length > 1) 
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject); // 確保跨 Scene 存在
    }

    void Start()
    {
        // Start 函數不再自動啟動 IntroRoutine 或 ShowIntroScreen
        // 遊戲狀態和 UI 顯示將主要由 OnSceneLoaded 方法處理
        // 這確保了 IntroManager 在 MainMenuScene 啟動時不會自動顯示 V.S. 畫面

        // 首次啟動時，確保 UI 引用被初始化 (針對 MainMenuScene 的情況)
        InitializeCurrentSceneUI();
        
        // 如果是 MainMenuScene (Build Index 0)，將遊戲狀態設定為 Paused，時間恢復正常
        if (SceneManager.GetActiveScene().buildIndex == 0) // Build Index 0 應該是 MainMenuScene
        {
            Time.timeScale = 1f; // 主選單是可互動的
            currentGameState = GameState.Paused; // 主選單是暫停狀態，等待玩家點擊 Game Start
        }
        // 其他 Scene 的啟動邏輯會由 OnSceneLoaded 處理
    }

    // 當 Scene 載入完成時，重新初始化 UI 引用並設定遊戲狀態
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeCurrentSceneUI(); // 初始化當前 Scene 的 UI 引用 (查找 IntroPanel, StartTextPanel, GameOverPanel)

        // 根據載入的 Scene 名稱，決定是否啟動 V.S. 畫面流程
        // V.S. 畫面現在只在 Level3GameScene (遊戲三) 開頭顯示
        if (GameProgressionManager.instance != null && scene.name == GameProgressionManager.instance.gameScenes[2]) // 假設 gameScenes[2] 是 Level3GameScene
        {
            Debug.Log("IntroManager: 偵測到第三關遊戲 Scene。啟動 V.S. 畫面流程。");
            Time.timeScale = 0f; // 暫停遊戲
            currentGameState = GameState.Intro; // 設定狀態為 Intro
            StartCoroutine(IntroRoutine()); // 啟動 V.S. 畫面流程
        }
        else // 其他遊戲關卡、大廳、商店、劇情 Scene 等，直接進入 Playing 狀態
        {
            Debug.Log($"IntroManager: 載入 Scene '{scene.name}'。遊戲進入 Playing 狀態。");
            Time.timeScale = 1f; // 恢復遊戲時間
            currentGameState = GameState.Playing; // 遊戲直接進入 Playing 狀態
            // 確保所有 Intro/Start UI 都是隱藏的
            if (currentIntroPanel != null) currentIntroPanel.SetActive(false);
            if (currentStartTextPanel != null) currentStartTextPanel.SetActive(false);
        }

        // 確保 GameOverPanel 和 LoserText 始終是隱藏的，直到被 ShowGameOver 呼叫
        if (currentGameOverPanel != null) currentGameOverPanel.SetActive(false);
        if (loserTextUI != null) loserTextUI.gameObject.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(false); 

        // 為「再玩一次」按鈕添加監聽器 (確保按鈕存在)
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners(); 
            restartButton.onClick.AddListener(RestartGame); // <<<< 這裡綁定 RestartGame 方法 >>>>>
        }
    }

    // 訂閱 Scene 加載事件 (在物件啟用時)
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 取消訂閱 (在物件禁用或銷毀時，防止空引用錯誤)
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 初始化當前 Scene 的 UI 引用 (根據名稱查找 Canvas 下的 Panel)
    void InitializeCurrentSceneUI()
    {
        Canvas currentCanvas = FindFirstObjectByType<Canvas>(); 

        if (currentCanvas != null)
        {
            // 查找 IntroPanel
            Transform introPanelTransform = currentCanvas.transform.Find(introPanelName);
            if (introPanelTransform != null) currentIntroPanel = introPanelTransform.gameObject; else currentIntroPanel = null;

            // 查找 StartTextPanel
            Transform startTextPanelTransform = currentCanvas.transform.Find(startTextPanelName);
            if (startTextPanelTransform != null) currentStartTextPanel = startTextPanelTransform.gameObject; else currentStartTextPanel = null;

            // 查找 GameOverPanel 及其子物件
            Transform gameOverPanelTransform = currentCanvas.transform.Find(gameOverPanelName);
            if (gameOverPanelTransform != null)
            {
                currentGameOverPanel = gameOverPanelTransform.gameObject;
                // 從 currentGameOverPanel 下查找子物件
                loserTextUI = currentGameOverPanel.GetComponentInChildren<TextMeshProUGUI>(true);
                restartButton = currentGameOverPanel.GetComponentInChildren<Button>(true);
            }
            else
            {
                currentGameOverPanel = null;
                loserTextUI = null; // 如果 GameOverPanel 不存在，則這些引用也設為 null
                restartButton = null;
            }
        }
        else
        {
            Debug.LogWarning("IntroManager: 當前 Scene 未找到 Canvas！", this);
            currentIntroPanel = null; currentStartTextPanel = null; currentGameOverPanel = null; loserTextUI = null; restartButton = null;
        }

        // 確保所有這些 Panel 在初始化後都是隱藏的，直到被特定邏輯激活
        if (currentIntroPanel != null) currentIntroPanel.SetActive(false);
        if (currentStartTextPanel != null) currentStartTextPanel.SetActive(false);
        if (currentGameOverPanel != null) currentGameOverPanel.SetActive(false);
        if (loserTextUI != null) loserTextUI.gameObject.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(false);
    }

    // IntroRoutine 協程負責顯示 V.S. 畫面和 START 文字
    IEnumerator IntroRoutine()
    {
        // 確保 UI 引用已在協程啟動時被初始化 (以防萬一)
        if (currentIntroPanel == null && currentStartTextPanel == null)
        {
            InitializeCurrentSceneUI();
        }

        if (currentIntroPanel != null) currentIntroPanel.SetActive(true);
        else Debug.LogError($"IntroManager: 無法顯示 Intro Panel '{introPanelName}'，請檢查名稱及是否存在於當前 Scene。", this);

        Debug.Log("步驟 1: 顯示 V.S. 圖片畫面。遊戲暫停中。");
        yield return new WaitForSecondsRealtime(vsScreenDuration);

        if (currentIntroPanel != null) currentIntroPanel.SetActive(false);

        if (currentStartTextPanel != null) currentStartTextPanel.SetActive(true);
        else Debug.LogError($"IntroManager: 無法顯示 Start Text Panel '{startTextPanelName}'，請檢查名稱及是否存在於當前 Scene。", this);

        // 獲取 StartTextPanel 下的 TextMeshProUGUI 元件並設定文字
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

    // 當 Player 死亡或其他遊戲結束條件滿足時，外部腳本 (例如 PlayerHealth) 會呼叫此方法
    public static void ShowGameOver()
    {
        if (currentGameState != GameState.GameOver)
        {
            Time.timeScale = 0f;
            currentGameState = GameState.GameOver;

            IntroManager instance = FindFirstObjectByType<IntroManager>();
            if (instance != null && instance.currentGameOverPanel != null)
            {
                instance.currentGameOverPanel.SetActive(true);
                Debug.Log("IntroManager: 顯示遊戲結束畫面。");
                
                instance.StartCoroutine(instance.ShowLoserTextFlow()); // 啟動流程
            }
            else
            {
                Debug.LogError($"IntroManager: 無法顯示遊戲結束畫面！請確保當前 Scene 有名為 '{instance?.gameOverPanelName}' 的 Panel。", instance);
            }
        }
    }

    // 協程：控制游戏结束画面的渐变、文字和按钮显示
    IEnumerator ShowLoserTextFlow()
    {
        // 1. 启动画面渐变
        FadeEffect fadeEffect = currentGameOverPanel?.GetComponentInChildren<FadeEffect>(true);
        if (fadeEffect != null)
        {
            fadeEffect.StartFadeIn(); 
            yield return new WaitForSecondsRealtime(fadeEffect.fadeDuration); 
        }
        else
        {
            Debug.LogWarning("IntroManager: 未找到 GameOverPanel 下的 FadeEffect 腳本！畫面不會漸變。", currentGameOverPanel);
            yield return new WaitForSecondsRealtime(gameOverFadeDelay); 
        }

        // 2. 显示 LoserText
        if (loserTextUI != null)
        {
            loserTextUI.gameObject.SetActive(true); 
            Debug.Log("IntroManager: LoserText 已顯示。");
        }
        else
        {
            Debug.LogWarning("IntroManager: LoserText UI 未連結！無法顯示遊戲結束文字。", this);
        }

        // 3. 显示「再玩一次」按钮
        yield return new WaitForSecondsRealtime(0.5f); 
        if (restartButton != null) 
        {
            restartButton.gameObject.SetActive(true); 
            Debug.Log("IntroManager: 再玩一次按鈕已顯示。");
        }
        else
        {
            Debug.LogWarning("IntroManager: 再玩一次按鈕未連結！無法顯示。", this);
        }
    }

    // <<<< 這個方法就是 RestartGame()，它的名字被錯貼了 >>>>>>
    public void RestartGame() // <<<< 這裡應該是 void RestartGame() >>>>>>
    {
        Debug.Log("IntroManager: '再玩一次' 按鈕被點擊。"); 
        Time.timeScale = 1f; 
        currentGameState = GameState.Playing; 

        GameProgressionManager.ResetProgress(); 
        
        // 設置 CurrentLevelIndex 到遊戲三的索引
        // 這裡不能直接設置 CurrentLevelIndex，因為它是只讀的
        // 應該由 GameProgressionManager 內部的方法來管理
        // 我們讓 GameProgressionManager 提供一個方法來重新開始指定關卡
        
        // 最簡單的回到遊戲三的開頭的邏輯：
        SceneManager.LoadScene(GameProgressionManager.instance.gameScenes[2]); // 直接加載 Level3GameScene (遊戲三)
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