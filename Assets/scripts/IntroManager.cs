using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class IntroManager : MonoBehaviour
{
    [Header("UI Panel 引用")]
    public GameObject introPanel;
    public GameObject startTextPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI startText; // ✅ 新增：直接拖入文字元件

    [Header("時間設定")]
    public float vsScreenDuration = 5f;
    public float startTextDuration = 2f;

    public enum GameState
    {
        Intro,
        Starting,
        Playing,
        Paused,
        GameOver
    }

    public static GameState currentGameState = GameState.Intro;

    void Start()
    {
        // 防呆檢查
        if (introPanel == null) Debug.LogError("❌ introPanel 未設定！");
        if (startTextPanel == null) Debug.LogError("❌ startTextPanel 未設定！");
        if (gameOverPanel == null) Debug.LogError("❌ gameOverPanel 未設定！");
        if (startText == null) Debug.LogError("❌ startText (TextMeshProUGUI) 未設定！");

        // 預先隱藏面板
        if (introPanel != null) introPanel.SetActive(false);
        if (startTextPanel != null) startTextPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 0f;
        currentGameState = GameState.Intro;

        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        if (introPanel != null) introPanel.SetActive(true);
        Debug.Log("▶️ 顯示 VS 畫面");

        yield return new WaitForSecondsRealtime(vsScreenDuration);

        if (introPanel != null) introPanel.SetActive(false);
        if (startTextPanel != null) startTextPanel.SetActive(true);

        if (startText != null)
        {
            startText.text = "GAME START !";
            startText.gameObject.SetActive(true);
        }

        Debug.Log("▶️ 顯示 START 字樣");
        yield return new WaitForSecondsRealtime(startTextDuration);

        if (startTextPanel != null) startTextPanel.SetActive(false);

        Debug.Log("▶️ 遊戲開始！");
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
            if (instance != null && instance.gameOverPanel != null)
            {
                instance.gameOverPanel.SetActive(true);
                Debug.Log("☠️ 顯示遊戲結束畫面");
            }
            else
            {
                Debug.LogError("❌ 無法顯示遊戲結束畫面，請確認 Inspector 設定！");
            }
        }
    }

    public static void PauseGame()
    {
        if (currentGameState == GameState.Playing)
        {
            Time.timeScale = 0f;
            currentGameState = GameState.Paused;
            Debug.Log("⏸️ 遊戲暫停");
        }
    }

    public static void ResumeGame()
    {
        if (currentGameState == GameState.Paused)
        {
            Time.timeScale = 1f;
            currentGameState = GameState.Playing;
            Debug.Log("▶️ 遊戲恢復");
        }
    }
}
