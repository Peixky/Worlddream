using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果按鈕是 TextMeshPro
using UnityEngine.SceneManagement;

public class StoreController : MonoBehaviour
{
    public Button nextStageButton;
    public Button buyButton;
    public Health playerHealth;
    void Start()
    {
        
        Time.timeScale = 1f;
        IntroManager.currentGameState = IntroManager.GameState.Playing;

        if (nextStageButton != null)
        {
            nextStageButton.onClick.AddListener(OnNextStageButtonClick);
        }

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(Buy);
        }
    }

    void OnNextStageButtonClick()
    {
        if (GameProgressionManager.CurrentLevelIndex == 0) 
        {
            GameProgressionManager.AdvanceLevel(); 
            GameProgressionManager.AdvanceStory(); 
            GameProgressionManager.LoadNextStoryScene(); 
        }
        else if (GameProgressionManager.CurrentLevelIndex == 1)
        {
            GameProgressionManager.AdvanceLevel();
            GameProgressionManager.AdvanceStory();
            GameProgressionManager.LoadNextStoryScene(); 
        }

    }

    void Buy()
    {
        if (CoinManager.instance.GetCoinCount() >= 3)
        {
            Debug.Log("購買");
            CoinManager.instance.RemoveCoin(3);
            playerHealth.IncreaseMaxHealth(1);
        }
    }
}