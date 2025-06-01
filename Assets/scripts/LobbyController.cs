using UnityEngine;
using UnityEngine.UI; // 用於 Button
using TMPro; // 如果你使用 TextMeshPro

public class LobbyController : MonoBehaviour
{
    [Header("UI 元素引用")]
    public Button nextStoryButton; // 拖曳你的「進入下段劇情」按鈕到這裡
    // 你可能還會有一個「當前進度」的 TextMeshProUGUI 來顯示例如「目前劇情：劇情X」

    void Awake()
    {
        // 確保遊戲時間是恢復的，以便大廳可以互動
        Time.timeScale = 1f; 

        // 綁定按鈕事件
        if (nextStoryButton != null)
        {
            nextStoryButton.onClick.RemoveAllListeners(); // 清除舊的監聽器
            nextStoryButton.onClick.AddListener(OnNextStoryButtonClick);
        }
        else
        {
            Debug.LogError("LobbyController: nextStoryButton 未連結！請在 Inspector 中設定。", this);
        }

        // 在大廳場景中，可以顯示當前遊戲進度
        DisplayCurrentProgress();
    }

    void DisplayCurrentProgress()
    {
        if (GameProgressionManager.instance != null)
        {
            Debug.Log($"Lobby: 目前劇情索引: {GameProgressionManager.CurrentStoryIndex}, 目前關卡索引: {GameProgressionManager.CurrentLevelIndex}");
            // 如果你有 TextMeshProUGUI 來顯示進度，可以在這裡更新它
            // 例如：progressText.text = $"目前劇情：{GameProgressionManager.CurrentStoryIndex + 1}";
        }
        else
        {
            Debug.LogWarning("LobbyController: GameProgressionManager 未初始化。無法顯示進度。");
        }
    }

    // 當「進入下段劇情」按鈕被點擊時，此方法會被呼叫
    public void OnNextStoryButtonClick()
    {
        Debug.Log("Lobby: 「進入下段劇情」按鈕被點擊！");

        if (GameProgressionManager.instance != null)
        {
            // 我們希望進入「下一個劇情」，所以先推進劇情索引
            // 然後載入這個新的劇情場景
            GameProgressionManager.AdvanceStory();
            GameProgressionManager.LoadNextStoryScene();
        }
        else
        {
            Debug.LogError("LobbyController: GameProgressionManager 尚未初始化！無法進入下段劇情。");
            // 作為備用，如果 GameProgressionManager 沒準備好，至少載入一個預設的劇情場景
            // 但這會繞過進度管理，通常不建議
            // SceneFlowManager.Instance.nextSceneName = "Story/Scene/劇情三"; // 假設下一個是劇情三
            // SceneFlowManager.Instance.GoToNextScene();
        }
    }
}