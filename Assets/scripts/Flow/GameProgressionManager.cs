using UnityEngine;
using UnityEngine.SceneManagement; // 用於加載 Scene

public class GameProgressionManager : MonoBehaviour
{
    // PlayerPrefs 儲存進度的 Key
    private const string CURRENT_STORY_KEY = "CurrentStoryIndex";
    private const string CURRENT_LEVEL_KEY = "CurrentLevelIndex";

    // 靜態屬性，方便從任何地方讀取當前進度
    // private set 確保只能在 GameProgressionManager 內部修改這些值
    public static int CurrentStoryIndex { get; private set; } // 0: 劇情一, 1: 劇情二, ...
    public static int CurrentLevelIndex { get; private set; } // 0: 第一關, 1: 第二關, ...

    // 場景名稱設定 (在 Inspector 中設定，必須與 Build Settings 中的 Scene 名稱完全一致)
    [Header("場景名稱設定 (與 Build Settings 匹配)")]
    // 範例值：{"Story/Scene/開場劇情", "Story/Scene/劇情二", "Story/Scene/劇情四"}
    public string[] storyScenes; 
    // 範例值：{"Scene 1", "Scene 2", "Scene 3"}
    public string[] gameScenes;  
    public string lobbySceneName = "LobbyScene"; // 大廳 Scene 的名稱
    public string endingSceneName = "EndingScene"; // 結局 Scene 的名稱 (如果有的話)

    // 單例模式：確保這個腳本在遊戲中只有一個實例，且不會被銷毀
    public static GameProgressionManager instance;

    void Awake()
    {
        // 檢查是否已經存在一個實例
        if (instance == null)
        {
            instance = this; // 如果這是第一個實例，則設定為唯一實例
            DontDestroyOnLoad(gameObject); // 確保在 Scene 切換時不被銷毀
            Debug.Log("GameProgressionManager: 單例實例已建立並設定為 DontDestroyOnLoad。");
        }
        else if (instance != this)
        {
            // 如果已經存在其他實例（可能從其他場景載入），則銷毀自己，確保唯一性
            Debug.LogWarning("GameProgressionManager: 檢測到重複實例，銷毀自己以保持單例模式。");
            Destroy(gameObject); 
            return; // 銷毀後立即返回，不執行後續代碼
        }

        LoadProgress(); // 加載之前儲存的進度
        
        // 遊戲啟動時，自動加載流程中的第一個 Scene (只有在遊戲的「真正起點」執行)
        // 根據你的流程，這個 StartGameFlow() 應該由主選單的按鈕來呼叫，所以這裡的自動觸發可以保留也可以移除，
        // 但為了避免重複觸發，通常會把這個邏輯放在主選單按鈕的 OnClick 事件裡。
        // 如果你的遊戲就是從 build index 0 直接開始進度，可以保留；否則，請確保MainMenuController呼叫它。
        /*
        if (SceneManager.GetActiveScene().buildIndex == 0) // 如果當前 Scene 是 Build Settings 中的第一個 (MainMenuScene)
        {
           StartGameFlow(); // 啟動整個遊戲流程，從劇情一開始
        }
        */
    }

    // 從 PlayerPrefs 加載遊戲進度
    void LoadProgress()
    {
        CurrentStoryIndex = PlayerPrefs.GetInt(CURRENT_STORY_KEY, 0); // 預設從劇情一 (索引 0) 開始
        CurrentLevelIndex = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0); // 預設從第一關 (索引 0) 開始
        Debug.Log($"GameProgressionManager: 進度加載：目前劇情索引 {CurrentStoryIndex}, 目前關卡索引 {CurrentLevelIndex}");
    }

    // 保存當前遊戲進度到 PlayerPrefs
    void SaveProgress()
    {
        PlayerPrefs.SetInt(CURRENT_STORY_KEY, CurrentStoryIndex);
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, CurrentLevelIndex);
        PlayerPrefs.Save(); 
        Debug.Log($"GameProgressionManager: 進度保存：目前劇情索引 {CurrentStoryIndex}, 目前關卡索引 {CurrentLevelIndex}");
    }

    // 推進劇情到下一段 (由 DialogueManager 或 LevelExitPoint 呼叫)
    public static void AdvanceStory()
    {
        if (instance == null) {
            Debug.LogError("GameProgressionManager instance is null! Cannot advance story.");
            return;
        }
        CurrentStoryIndex++;
        instance.SaveProgress(); // 保存進度
        Debug.Log($"GameProgressionManager: 劇情推進到：{CurrentStoryIndex}");
    }

    // 推進關卡到下一關 (由 LobbyController 呼叫)
    public static void AdvanceLevel()
    {
        if (instance == null) {
            Debug.LogError("GameProgressionManager instance is null! Cannot advance level.");
            return;
        }
        CurrentLevelIndex++;
        instance.SaveProgress(); // 保存進度
        Debug.Log($"GameProgressionManager: 關卡推進到：{CurrentLevelIndex}");
    }

    // 啟動整個遊戲流程 (通常由主選單的 "Game Start" 按鈕呼叫)
    public static void StartGameFlow()
    {
        if (instance == null) {
            Debug.LogError("GameProgressionManager instance is null! Cannot start game flow. Please ensure it's loaded and initialized.");
            return;
        }
        // 重置所有進度 (確保從頭開始玩)
        ResetProgress(); 
        CurrentStoryIndex = 0; // 確保從第一個劇情開始
        CurrentLevelIndex = 0; // 確保從第一關開始
        instance.SaveProgress(); // 保存初始進度
        Debug.Log("GameProgressionManager: 遊戲流程已啟動，將加載第一段劇情。");
        LoadNextStoryScene(); // 從第一段劇情 Scene (IntroScene) 開始
    }

    // 加載下一段劇情 Scene
    public static void LoadNextStoryScene()
    {
        if (instance == null) {
            Debug.LogError("GameProgressionManager instance is null! Cannot load next story scene.");
            return;
        }
        if (CurrentStoryIndex < instance.storyScenes.Length)
        {
            Debug.Log($"GameProgressionManager: 加載劇情 Scene: {instance.storyScenes[CurrentStoryIndex]} (索引 {CurrentStoryIndex})");
            SceneManager.LoadScene(instance.storyScenes[CurrentStoryIndex]);
        }
        else
        {
            Debug.Log("GameProgressionManager: 所有劇情已結束！加載遊戲結局 Scene。");
            // 如果所有劇情都結束了 (例如劇情四之後)，加載結局畫面
            SceneManager.LoadScene(instance.endingSceneName); // 加載結局 Scene
        }
    }

    // 加載下一關遊戲 Scene
    public static void LoadNextGameScene()
    {
        if (instance == null) {
            Debug.LogError("GameProgressionManager instance is null! Cannot load next game scene.");
            return;
        }
        if (CurrentLevelIndex < instance.gameScenes.Length)
        {
            Debug.Log($"GameProgressionManager: 加載關卡 Scene: {instance.gameScenes[CurrentLevelIndex]} (索引 {CurrentLevelIndex})");
            SceneManager.LoadScene(instance.gameScenes[CurrentLevelIndex]);
        }
        else
        {
            Debug.Log("GameProgressionManager: 所有關卡已結束！加載最終劇情或結束畫面。");
            // 如果所有關卡都打完了，加載最後一段劇情 (假設是最後一個劇情)
            CurrentStoryIndex = instance.storyScenes.Length - 1; // 設定為最後一個劇情索引
            LoadNextStoryScene(); // 加載最後一段劇情 (例如劇情四)
        }
    }

    // 加載大廳 Scene
    public static void LoadLobbyScene()
    {
        if (instance == null) {
            Debug.LogError("GameProgressionManager instance is null! Cannot load lobby scene.");
            return;
        }
        Debug.Log($"GameProgressionManager: 加載大廳 Scene: {instance.lobbySceneName}");
        SceneManager.LoadScene(instance.lobbySceneName);
    }
    
    // 重置所有進度 (用於測試或遊戲重玩)
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(CURRENT_STORY_KEY);
        PlayerPrefs.DeleteKey(CURRENT_LEVEL_KEY);
        PlayerPrefs.Save();
        // 重置靜態變數
        CurrentStoryIndex = 0;
        CurrentLevelIndex = 0;
        Debug.Log("GameProgressionManager: 遊戲進度已重置。");
    }
}