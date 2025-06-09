using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果按鈕是 TextMeshPro
using UnityEngine.SceneManagement;

public class StoreController : MonoBehaviour
{
    public Button nextStageButton; 
    public Button buyButton;      

    void Start()
    {
        Time.timeScale = 1f; 
        GameProgressionManager.currentGameState = GameProgressionManager.GameState.Paused; // <<<< 修正 >>>>

        if (nextStageButton != null)
        {
            nextStageButton.onClick.RemoveAllListeners(); // 確保不會重複綁定
            nextStageButton.onClick.AddListener(OnNextStageButtonClick);
        }
        else
        {
            Debug.LogError("StoreController: NextStageButton 未連結！請在 Inspector 中設定。", this);
        }

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners(); // 確保不會重複綁定
            buyButton.onClick.AddListener(Buy);
        }
        else
        {
            Debug.LogError("StoreController: BuyButton 未連結！請在 Inspector 中設定。", this);
        }
    }

    void OnNextStageButtonClick()
    {
        Debug.Log("商店：下一關按鈕被點擊。");
        GameProgressionManager.AdvanceLevel(); 
        GameProgressionManager.AdvanceStory(); 
        GameProgressionManager.LoadNextStoryScene();
    }

    void Buy()
    {
        int purchaseCost = 3; // 購買所需金幣數
        int maxHealthIncreaseAmount = 1; // 每次購買增加的最大血量上限

        if (CoinManager.instance != null && CoinManager.instance.GetCoinCount() >= purchaseCost)
        {
            Debug.Log($"商店：嘗試購買最大血量，花費 {purchaseCost} 金幣。");
            CoinManager.instance.RemoveCoin(purchaseCost);

            GameProgressionManager.SetPlayerMaxHealth(GameProgressionManager.PlayerMaxHealth + maxHealthIncreaseAmount); // <<<< 修正 >>>>

            Debug.Log("商店：成功購買最大血量上限！");
        }
        else
        {
            Debug.Log("商店：金幣不足，無法購買最大血量！");
        }
    }
}