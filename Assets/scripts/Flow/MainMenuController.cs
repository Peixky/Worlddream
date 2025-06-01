using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Button startGameButton; // 拖曳 StartGameButton UI 元件到這裡

    void Start()
    {
        // 確保遊戲時間是恢復的，以便主選單可以互動
        Time.timeScale = 1f;
        // 主選單通常是 Paused 狀態，等待玩家點擊
        // IntroManager.currentGameState = IntroManager.GameState.Paused; // 確保 IntroManager 存在且此行有意義

        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameButtonClick);
        }
        else
        {
            Debug.LogError("MainMenuController: StartGameButton 未連結！請在 Inspector 中設定。", this);
        }

        // 確保 GameProgressionManager 存在並啟動 (它應該由 GameManager 物件攜帶)
        if (FindObjectOfType<GameProgressionManager>() == null)
        {
            Debug.LogWarning("MainMenuController: GameProgressionManager 未找到，請確保 GameManager 物件在 MainMenuScene 中。");
        }
    }

    // 當 "Game Start" 按鈕被點擊時，此方法會被呼叫
    // <<<< 這裡！加上 public 關鍵字 >>>>
    public void OnStartGameButtonClick()
    {
        Debug.Log("Game Start 按鈕被點擊！");
        // 呼叫 GameProgressionManager 來啟動遊戲流程
        // 這將會加載第一個劇情 Scene (IntroScene)
        if (GameProgressionManager.instance != null) // 再次加上 null 檢查以防萬一
        {
            GameProgressionManager.StartGameFlow();
        }
        else
        {
            Debug.LogError("GameProgressionManager 尚未初始化！無法啟動遊戲流程。請檢查 GameProgressionManager 的載入順序或 GameObject 設定。", this);
            // 作為備用，如果 GameProgressionManager 沒準備好，至少先切場景以防卡住
            // 這部分邏輯應該由 GameProgressionManager 自己處理，所以這裡可以只報錯
            // 或根據你的設計決定是否需要備用切換
            // SceneFlowManager.Instance.nextSceneName = "Story/Scene/開場劇情";
            // SceneFlowManager.Instance.GoToNextScene();
        }
    }
}