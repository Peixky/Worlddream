using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果按鈕是 TextMeshPro
using UnityEngine.SceneManagement; 

public class StoreController : MonoBehaviour
{
    public Button nextStageButton; // 拖曳您的「下一關」按鈕 UI 元件到這裡

    void Start()
    {
        Time.timeScale = 1f; 
        IntroManager.currentGameState = IntroManager.GameState.Playing; 

        if (nextStageButton != null)
        {
            nextStageButton.onClick.AddListener(OnNextStageButtonClick);
        }
        else
        {
            Debug.LogError("StoreController: NextStageButton 未連結！請在 Inspector 中設定。", this);
        }
    }

    void OnNextStageButtonClick()
    {
        Debug.Log("商店：下一關按鈕被點擊。");
        // 商店結束後，根據流程圖，判斷下一步是劇情二還是劇情三
        // 這是遊戲邏輯中需要判斷的點
        if (GameProgressionManager.CurrentLevelIndex == 0) // 從 Level1 進入商店
        {
            GameProgressionManager.AdvanceLevel(); // 推進到 Level2
            GameProgressionManager.AdvanceStory(); // 推進到 StoryScene2
            GameProgressionManager.LoadNextStoryScene(); // 加載 StoryScene2
        }
        else if (GameProgressionManager.CurrentLevelIndex == 1) // 從 Level2 進入商店
        {
            GameProgressionManager.AdvanceLevel();
            GameProgressionManager.AdvanceStory(); // 推進到 StoryScene3
            GameProgressionManager.LoadNextStoryScene(); // 加載 StoryScene3
        }
        // 如果有其他商店進入點，需要根據 CurrentLevelIndex 或 CurrentStoryIndex 判斷
    }
}