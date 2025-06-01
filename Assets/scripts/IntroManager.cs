using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.Collections; 
using System; 
using UnityEngine.SceneManagement; // 確保有這個 using

public class IntroManager : MonoBehaviour
{
    // UI Panel 引用 (這些 Panel 需要您在 Canvas 下創建並拖曳到 Inspector)
    [Header("UI Panel 引用")]
    public GameObject introPanel;      // 用於顯示 Player vs Boss 圖片的 Panel
    public GameObject startTextPanel;  // 用於顯示 "START" 文字的 Panel
    public GameObject gameOverPanel;   // 用於顯示遊戲結束畫面的 Panel

    // 時間設定
    [Header("時間設定")]
    public float vsScreenDuration = 5f; // V.S. 畫面停留時間 (秒)
    public float startTextDuration = 2f; // "START" 文字停留時間 (秒)

    // 遊戲狀態的枚舉 (Enum)，方便其他腳本判斷遊戲當前所處的階段
    public enum GameState
    {
        Intro,      // 遊戲介紹中 (V.S. 圖片顯示)
        Starting,   // 準備開始 ("START" 文字顯示)
        Playing,    // 遊戲正在進行中
        Paused,     // 遊戲暫停
        GameOver    // 遊戲結束
    }

    public static GameState currentGameState = GameState.Intro; // 靜態變數，初始為 Intro

    void Start()
    {
        // 確保所有 UI Panel 在遊戲開始時都是隱藏的 (即使在 Inspector 中設定了，這裡也再次確認)
        // 這樣可以避免在 Intro 流程開始前，UI 就提前顯示出來
        if (introPanel != null) introPanel.SetActive(false);
        if (startTextPanel != null) startTextPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false); 

        // 遊戲開始時，先將遊戲時間暫停
        Time.timeScale = 0f; 
        currentGameState = GameState.Intro;

        // 遊戲啟動時，由 GameProgressionManager 負責加載正確的 Scene
        // 如果目前在 IntroScene，則自動顯示 Intro 畫面
        if (SceneManager.GetActiveScene().name == GameProgressionManager.instance.storyScenes[0])
        {
            StartCoroutine(IntroRoutine());
        }
        else // 如果不在 IntroScene (例如從 Level 或 Lobby 進入)
        {
            Time.timeScale = 1f; // 恢復遊戲時間
            currentGameState = GameState.Playing; // 遊戲直接進入 Playing 狀態
        }
    }

    // 這個方法由外部 (例如 GameProgressionManager) 呼叫來顯示 IntroScreen
    public void ShowIntroScreen()
    {
        Debug.Log("IntroManager: ShowIntroScreen 被呼叫，啟動開場協程。");
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        // 1. 顯示 V.S. 畫面 (激活 introPanel)
        // 確保 introPanel 存在才執行操作
        if (introPanel != null) introPanel.SetActive(true);
        else Debug.LogError("IntroManager: introPanel 未連結！請在 Inspector 中設定。", this);
        
        Debug.Log("步驟 1: 顯示 V.S. 圖片畫面。遊戲暫停中。");
        // 使用 WaitForSecondsRealtime 確保在 Time.timeScale=0f 時也能正常計時
        yield return new WaitForSecondsRealtime(vsScreenDuration); // 等待設定的 V.S. 畫面顯示時間

        // 2. 隱藏 V.S. 畫面，顯示 "START" 文字 (激活 startTextPanel)
        if (introPanel != null) introPanel.SetActive(false); 

        if (startTextPanel != null) startTextPanel.SetActive(true); 
        else Debug.LogError("IntroManager: startTextPanel 未連結！請在 Inspector 中設定。", this);

        // 獲取 startTextPanel 下的 TextMeshProUGUI 元件並設定文字
        // GetComponentInChildren 會尋找子物件中的元件，即使子物件是隱藏的也能找到
        TextMeshProUGUI startTextMesh = startTextPanel?.GetComponentInChildren<TextMeshProUGUI>(true); 
        if (startTextMesh != null)
        {
            startTextMesh.text = "START";
            // 確保 TextMeshProUGUI 元件也是激活的
            if (!startTextMesh.gameObject.activeSelf) startTextMesh.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("IntroManager: 未找到 StartTextPanel 下的 TextMeshProUGUI 元件！", startTextPanel);
        }
        
        Debug.Log("步驟 2: V.S. 圖片消失延遲結束。顯示 START 文字。");
        yield return new WaitForSecondsRealtime(startTextDuration); 

        // 3. 隱藏 "START" 文字，恢復遊戲
        if (startTextPanel != null) startTextPanel.SetActive(false);
        Debug.Log("步驟 3: START 文字消失。遊戲開始！");

        Time.timeScale = 1f; 
        currentGameState = GameState.Playing; 

        // 觸發 GameEvents.OnGameStart 事件，通知其他腳本 (例如 BossController) 可以開始其行為
        GameEvents.OnGameStart?.Invoke(); 
    }

    // 當 Player 死亡或其他遊戲結束條件滿足時，外部腳本 (例如 PlayerHealth) 會呼叫此方法
    public static void ShowGameOver()
    {
        if (currentGameState != GameState.GameOver) 
        {
            Time.timeScale = 0f; 
            currentGameState = GameState.GameOver; 

            // 尋找場景中唯一一個 IntroManager 的實例來操作其 UI
            // 因為這是靜態方法，無法直接訪問非靜態變數，所以需要 FindFirstObjectByType
            IntroManager instance = FindFirstObjectByType<IntroManager>();
            if (instance != null && instance.gameOverPanel != null)
            {
                instance.gameOverPanel.SetActive(true); 
                Debug.Log("IntroManager: 顯示遊戲結束畫面。");
            }
            else
            {
                Debug.LogError("IntroManager: 無法顯示遊戲結束畫面！請確保場景中有 GameManager 物件且掛載 IntroManager 腳本，並將 gameOverPanel 拖曳到 Inspector。", instance);
            }
        }
    }

    // 遊戲暫停功能 (可以綁定到按鈕或按鍵)
    public static void PauseGame()
    {
        if (currentGameState == GameState.Playing)
        {
            Time.timeScale = 0f;
            currentGameState = GameState.Paused;
            Debug.Log("遊戲暫停。");
        }
    }

    // 遊戲恢復功能
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