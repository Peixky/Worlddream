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
            }
        }
    }

    public enum GameState { Intro, Playing, Paused, GameOver }
    public static GameState currentGameState = GameState.Intro;

    public enum GameFlowMode { Story, Level }
    public static GameFlowMode currentGameFlowMode = GameFlowMode.Story;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        LoadProgress(); 
        OnPlayerHealthChanged?.Invoke(); 
        OnPlayerCashChanged?.Invoke();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "level2" || scene.name == "Scene1")
        {
            GameObject exit = GameObject.Find("Levelexit");
            if (exit != null)
                exit.SetActive(false);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void LoadProgress()
    {
        CurrentStoryIndex = PlayerPrefs.GetInt(CURRENT_STORY_KEY, 0);
        CurrentLevelIndex = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0);
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(CURRENT_STORY_KEY, CurrentStoryIndex);
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, CurrentLevelIndex);
        PlayerPrefs.Save();
    }

    public static void AdvanceStory()
    {
        CurrentStoryIndex++;
        instance.SaveProgress(); 
    }

    public static void AdvanceLevel()
    {
        CurrentLevelIndex++;
        instance.SaveProgress(); 
    }

    public static void StartGameFlow()
    {
        currentGameFlowMode = GameFlowMode.Story;
        ResetProgress(); 
        instance.SaveProgress(); 
        LoadLobbyScene(); 
    }

    public static void StartLevelMode()
    {
        currentGameFlowMode = GameFlowMode.Level;
        LoadNextGameScene();
    }

    public static void LoadNextStoryScene()
    {
        if (instance == null) return;

        if (CurrentStoryIndex < instance.storyScenes.Length)
        {
            SceneManager.LoadScene(instance.storyScenes[CurrentStoryIndex]);
        }
        else
        {
            SceneManager.LoadScene(instance.endingSceneName);
        }
    }

    public static void LoadNextGameScene()
    {
        if (instance == null) return;

        if (CurrentLevelIndex < instance.gameScenes.Length)
        {
            SceneManager.LoadScene(instance.gameScenes[CurrentLevelIndex]);
        }
        else
        {
            if (currentGameFlowMode == GameFlowMode.Story && instance.storyScenes.Length > 0)
            {
                CurrentStoryIndex = instance.storyScenes.Length - 1;
                LoadNextStoryScene();
            }
            else
            {
                LoadLobbyScene();
            }
        }
    }

    public static void LoadLobbyScene()
    {
        if (instance == null) return;
        SceneManager.LoadScene(instance.lobbySceneName);
    }

    public static void LoadStoreScene()
    {
        if (instance == null) return;
        SceneManager.LoadScene(instance.storeSceneName);
    }

    public static void LoadBossDeathScene()
    {
        if (instance == null) return;
        SceneManager.LoadScene(instance.bossDeathSceneName);
    }

    public static void AddCash(int amount) { PlayerCash += amount; }
    public static bool SpendCash(int amount)
    {
        if (PlayerCash >= amount) { PlayerCash -= amount; return true; }
        return false;
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
        PlayerHealth = 3;
        PlayerCash = 0;
        PlayerMaxHealth = 10;

        OnPlayerHealthChanged?.Invoke();
        OnPlayerCashChanged?.Invoke();
    }

    public static void PauseGame()
    {
        if (currentGameState == GameState.Playing)
        {
            Time.timeScale = 0f;
            currentGameState = GameState.Paused;
        }
    }

    public static void ResumeGame()
    {
        if (currentGameState == GameState.Paused)
        {
            Time.timeScale = 1f;
            currentGameState = GameState.Playing;
        }
    }
}
