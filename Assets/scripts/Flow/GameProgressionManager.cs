using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameProgressionManager : MonoBehaviour
{
    private const string CURRENT_STORY_KEY = "CurrentStoryIndex";
    private const string CURRENT_LEVEL_KEY = "CurrentLevelIndex";
    private const string HEALTH_KEY = "PlayerHealth";
    private const string CASH_KEY = "PlayerCash";
    private const string MAX_HEALTH_KEY = "PlayerMaxHealth";

    public static int CurrentStoryIndex { get; private set; } 
    public static int CurrentLevelIndex { get; private set; } 

    
    public static event Action OnPlayerHealthChanged; 
    public static event Action OnPlayerCashChanged;   

    public static int PlayerHealth
    {
        get { return PlayerPrefs.GetInt(HEALTH_KEY, 3); } 
        set
        {
            int clampedHealth = Mathf.Clamp(value, 0, PlayerMaxHealth);
            if (PlayerPrefs.GetInt(HEALTH_KEY, 0) != clampedHealth)
            {
                PlayerPrefs.SetInt(HEALTH_KEY, clampedHealth);
                PlayerPrefs.Save(); 
                OnPlayerHealthChanged?.Invoke(); 
                Debug.Log($"GameProgressionManager: 玩家血量更新為: {clampedHealth}");
            }
        }
    }

    public static int PlayerCash
    {
        get { return PlayerPrefs.GetInt(CASH_KEY, 0); }
        set
        {
            if (PlayerPrefs.GetInt(CASH_KEY, 0) != value)
            {
                PlayerPrefs.SetInt(CASH_KEY, value);
                PlayerPrefs.Save(); 
                OnPlayerCashChanged?.Invoke(); 
                Debug.Log($"GameProgressionManager: 玩家金幣更新為: {value}");
            }
        }
    }

    public static int PlayerMaxHealth
    {
        get { return PlayerPrefs.GetInt(MAX_HEALTH_KEY, 10); } 
        set
        {
            if (PlayerPrefs.GetInt(MAX_HEALTH_KEY, 0) != value)
            {
                PlayerPrefs.SetInt(MAX_HEALTH_KEY, value);
                PlayerPrefs.Save(); 
                OnPlayerHealthChanged?.Invoke(); 
                Debug.Log($"GameProgressionManager: 玩家最大血量更新為: {value}");
            }
        }
    }

    public enum GameState
    {
        Intro,      
        Playing,   
        Paused,    
        GameOver   
    }

    public static GameState currentGameState = GameState.Intro; 

    
    [Header("場景名稱設定")]
    public string[] storyScenes; 
    public string[] gameScenes;  
    public string lobbySceneName = "LobbyScene"; 
    public string storeSceneName = "StoreScene"; 
    public string endingSceneName = "EndingScene"; 
    public string bossDeathSceneName = "BossDeathScene"; 

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
        OnPlayerHealthChanged?.Invoke(); 
        OnPlayerCashChanged?.Invoke();

        if (SceneManager.GetActiveScene().buildIndex == 0) 
        {
            Time.timeScale = 1f; 
            currentGameState = GameState.Paused; 
        }
        else
        {
            Time.timeScale = 1f;
            currentGameState = GameState.Playing;
        }
    }

    // === 進度讀取與保存 ===
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

    // === 進度推進方法 ===
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

    // === 遊戲流程控制 ===
    public static void StartGameFlow()
    {
        ResetProgress(); 
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
            if (instance.storyScenes.Length > 0)
            {
                CurrentStoryIndex = instance.storyScenes.Length - 1; 
                LoadNextStoryScene(); 
            }
            else
            {
                SceneManager.LoadScene(instance.endingSceneName); 
            }
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

    public static void LoadBossDeathScene() 
    {
        if (instance == null) return;
        Debug.Log($"加載 Boss 死亡劇情 Scene: {instance.bossDeathSceneName}"); // 使用新的變數名
        SceneManager.LoadScene(instance.bossDeathSceneName);
    }

   
    public static void AddCash(int amount) { PlayerCash += amount; }
    public static bool SpendCash(int amount) 
    { 
        if (PlayerCash >= amount) { PlayerCash -= amount; return true; } 
        Debug.Log("金幣不足！"); return false; 
    }
    public static void AddHealth(int amount) { PlayerHealth += amount; }
    public static void SetPlayerMaxHealth(int newMax) { PlayerMaxHealth = newMax; PlayerHealth = PlayerHealth; }

    
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(CURRENT_STORY_KEY);
        PlayerPrefs.DeleteKey(CURRENT_LEVEL_KEY);
        PlayerPrefs.DeleteKey(HEALTH_KEY);
        PlayerPrefs.DeleteKey(CASH_KEY);
        PlayerPrefs.DeleteKey(MAX_HEALTH_KEY);
        PlayerPrefs.Save(); 

        CurrentStoryIndex = 0;
        CurrentLevelIndex = 0;
        PlayerHealth = PlayerPrefs.GetInt(HEALTH_KEY, 3); 
        PlayerCash = PlayerPrefs.GetInt(CASH_KEY, 0);     
        PlayerMaxHealth = PlayerPrefs.GetInt(MAX_HEALTH_KEY, 10); 

        Debug.Log("遊戲進度與所有玩家數據已重置。");

        OnPlayerHealthChanged?.Invoke();
        OnPlayerCashChanged?.Invoke();
    }

    // === 從 IntroManager 移過來的全局暫停/恢復方法 ===
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