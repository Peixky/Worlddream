using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyController : MonoBehaviour
{
    [Header("UI 元素引用")]
    public Button nextStoryButton; 
    

    void Awake()
    {
        Time.timeScale = 1f; 


        if (nextStoryButton != null)
        {
            nextStoryButton.onClick.RemoveAllListeners();
            nextStoryButton.onClick.AddListener(OnNextStoryButtonClick);
        }
        else
        {
            Debug.LogError("LobbyController: nextStoryButton 未連結！請在 Inspector 中設定。", this);
        }

        DisplayCurrentProgress();
    }

    void DisplayCurrentProgress()
    {
        if (GameProgressionManager.instance != null)
        {
            Debug.Log($"Lobby: 目前劇情索引: {GameProgressionManager.CurrentStoryIndex}, 目前關卡索引: {GameProgressionManager.CurrentLevelIndex}");
            GameProgressionManager.ResetProgress();
        }
        else
        {
            Debug.LogWarning("LobbyController: GameProgressionManager 未初始化。無法顯示進度。");
        }
    }

    public void OnNextStoryButtonClick()
    {
        Debug.Log("Lobby: 「進入下段劇情」按鈕被點擊！");

        if (GameProgressionManager.instance != null)
        {
            GameProgressionManager.LoadNextStoryScene();
        }
        else
        {
            Debug.LogError("LobbyController: GameProgressionManager 尚未初始化！無法進入下段劇情。");
            // SceneFlowManager.Instance.nextSceneName = "Story/Scene/劇情三"; // 假設下一個是劇情三
            // SceneFlowManager.Instance.GoToNextScene();
        }
    }
}