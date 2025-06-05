using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

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

    public static GameProgressionManager.GameState currentGameState = GameProgressionManager.GameState.Intro;

    void Start()
    {
        Debug.Log($"IntroManager: 在 '{SceneManager.GetActiveScene().name}' 啟動。");

        if (introPanel != null) introPanel.SetActive(false);
        if (startTextPanel != null) startTextPanel.SetActive(false);
        if (fadePanel != null) fadePanel.SetActive(false);
        if (loserTextUI != null) loserTextUI.gameObject.SetActive(false);

        Time.timeScale = 0f;
        GameProgressionManager.currentGameState = GameProgressionManager.GameState.Intro;

        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        if (introPanel != null) introPanel.SetActive(true);
        Debug.Log("步驟 1: 顯示 V.S. 圖片畫面。遊戲暫停中。");

        yield return new WaitForSecondsRealtime(vsScreenDuration);

        if (introPanel != null) introPanel.SetActive(false);
        if (startTextPanel != null) startTextPanel.SetActive(true);

        var startText = startTextPanel.GetComponentInChildren<TextMeshProUGUI>(true);
        if (startText != null)
        {
            startText.text = "START";
            startText.gameObject.SetActive(true);
        }

        Debug.Log("步驟 2: 顯示 START 文字。");

        yield return new WaitForSecondsRealtime(startTextDuration);

        if (startTextPanel != null) startTextPanel.SetActive(false);
        Debug.Log("步驟 3: START 文字消失。遊戲開始！");

        Time.timeScale = 1f;
        GameProgressionManager.currentGameState = GameProgressionManager.GameState.Playing;
        GameEvents.OnGameStart?.Invoke();
    }

    public static void ShowGameOver()
    {
        var instance = FindFirstObjectByType<IntroManager>();
        if (instance == null) return;

        // 這個判斷確保 ShowGameOver 只在 Level3GameScene 中生效，否則不執行任何 UI 相關邏輯
        if (GameProgressionManager.instance != null && SceneManager.GetActiveScene().name == GameProgressionManager.instance.gameScenes[2])
        {
            if (GameProgressionManager.currentGameState != GameProgressionManager.GameState.GameOver)
            {
                Time.timeScale = 0f;
                GameProgressionManager.currentGameState = GameProgressionManager.GameState.GameOver;

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
    }

    IEnumerator ShowLoserTextFlow()
    {
        FadeEffect fadeEffect = null;
        if (fadePanel != null)
        {
            fadeEffect = fadePanel.GetComponentInChildren<FadeEffect>(true);
        }

        if (fadeEffect != null)
        {
            fadeEffect.StartFadeIn();
            yield return new WaitForSecondsRealtime(fadeEffect.fadeDuration);
        }
        else
        {
            Debug.LogWarning("IntroManager: FadeEffect 沒有設定或不存在！");
            yield return new WaitForSecondsRealtime(gameOverFadeDelay);
        }

        if (loserTextUI != null)
        {
            loserTextUI.gameObject.SetActive(true);
            Debug.Log("IntroManager: 顯示 LoserText！");
        }
        else
        {
            Debug.LogWarning("IntroManager: LoserTextUI 未設定！");
        }

        // === 修正開始：在顯示完 LoserText 後進入玩家死亡劇情 Scene ===
        // 在 LoserText 顯示後，等待一個短暫時間 (可選，讓玩家看清文字)
        yield return new WaitForSecondsRealtime(1.0f); // 等待 1 秒讓玩家讀取訊息

        Debug.Log("IntroManager: 玩家死亡流程結束，載入玩家死亡劇情 Scene。");
        GameProgressionManager.LoadPlayerDeathStoryScene(); // 呼叫載入玩家死亡劇情 Scene 的方法
        // === 修正結束 ===
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