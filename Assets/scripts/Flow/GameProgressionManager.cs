using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameProgressionManager : MonoBehaviour
{
    private const string CURRENT_STORY_KEY = "CurrentStoryIndex";
    private const string CURRENT_LEVEL_KEY = "CurrentLevelIndex";

    public static int CurrentStoryIndex { get; private set; } 
    public static int CurrentLevelIndex { get; private set; } 

    [Header("場景名稱設定 (與 Build Settings 匹配)")]
    public string[] storyScenes; 
    public string[] gameScenes;      public string lobbySceneName = "LobbyScene"; 
    public string storeSceneName = "StoreScene";
    public string endingSceneName = "EndingScene"; 

    public static GameProgressionManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else if (instance != this)
        {
            Destroy(gameObject); 
            return; 
        }

        LoadProgress();

        if (SceneManager.GetActiveScene().buildIndex == 0) 
        {
           // 等待 MainMenuController 點擊按鈕
        }
    }

    void LoadProgress()
    {
        CurrentStoryIndex = PlayerPrefs.GetInt(CURRENT_STORY_KEY, 0); 
        CurrentLevelIndex = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0); 
        Debug.Log($"進度加載：目前劇情索引 {CurrentStoryIndex}, 目前關卡索引 {CurrentLevelIndex}");
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(CURRENT_STORY_KEY, CurrentStoryIndex);
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, CurrentLevelIndex);
        PlayerPrefs.Save(); 
        Debug.Log($"進度保存：目前劇情索引 {CurrentStoryIndex}, 目前關卡索引 {CurrentLevelIndex}");
    }

    public static void AdvanceStory()
    {
        CurrentStoryIndex++;
        instance.SaveProgress();
        Debug.Log($"劇情推進到：{CurrentStoryIndex}");
    }

    public static void AdvanceLevel()
    {
        CurrentLevelIndex++;
        instance.SaveProgress();
        Debug.Log($"關卡推進到：{CurrentLevelIndex}");
    }

    public static void StartGameFlow()
    {
        ResetProgress(); 
        CurrentStoryIndex = 0;
        CurrentLevelIndex = 0;
        instance.SaveProgress();
        LoadLobbyScene();
    }

    public static void LoadNextStoryScene()
    {
        if (instance == null) return;
        if (CurrentStoryIndex < instance.storyScenes.Length)
        {
            Debug.Log($"加載劇情 Scene: {instance.storyScenes[CurrentStoryIndex]}");
            SceneManager.LoadScene(instance.storyScenes[CurrentStoryIndex]);
        }
        else
        {
            Debug.Log("所有劇情已結束！加載遊戲結束 Scene。");
            SceneManager.LoadScene(instance.endingSceneName); 
        }
    }

    public static void LoadNextGameScene()
    {
        if (instance == null) return;
        if (CurrentLevelIndex < instance.gameScenes.Length)
        {
            Debug.Log($"加載關卡 Scene: {instance.gameScenes[CurrentLevelIndex]}");
            SceneManager.LoadScene(instance.gameScenes[CurrentLevelIndex]);
        }
        else
        {
            Debug.Log("所有關卡已結束！加載最終劇情或結束畫面。");
            CurrentStoryIndex = instance.storyScenes.Length - 1; 
            LoadNextStoryScene(); 
        }
    }

    public static void LoadLobbyScene()
    {
        if (instance == null) return;
        Debug.Log($"加載大廳 Scene: {instance.lobbySceneName}");
        SceneManager.LoadScene(instance.lobbySceneName);
    }

    public static void LoadStoreScene()
    {
        if (instance == null) return;
        Debug.Log($"加載商店 Scene: {instance.storeSceneName}");
        SceneManager.LoadScene(instance.storeSceneName);
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(CURRENT_STORY_KEY);
        PlayerPrefs.DeleteKey(CURRENT_LEVEL_KEY);
        PlayerPrefs.Save();
        CurrentStoryIndex = 0;
        CurrentLevelIndex = 0;
        Debug.Log("遊戲進度已重置。");
    }
}