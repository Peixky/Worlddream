using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果按鈕是 TextMeshPro
using UnityEngine.SceneManagement;

public class StoreController : MonoBehaviour
{
    public Button nextStageButton; // 拖曳「下一關」按鈕 UI 元件到這裡
    public Button buyButton;       // 拖曳「購買」按鈕 UI 元件到這裡 (假設這是購買最大血量上限的按鈕)

    // 不需要直接引用 playerHealth.Health，因為我們將透過 GameProgressionManager 管理血量上限
    // Removed: public Health playerHealth; 

    void Start()
    {
        Time.timeScale = 1f; 
        // 修正這裡：商店場景通常是「暫停」狀態 (Paused)，但允許互動，而不是「遊戲中」(Playing)。
        // 並且，currentGameState 已經移至 GameProgressionManager。
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

        // 可以在這裡初始化商店 UI (例如顯示金幣和血量上限，這部分應在 ShopScript 中完成)
        // 確保 CoinManager 和 ShopScript 已經正確設置並工作
    }

    void OnNextStageButtonClick()
    {
        Debug.Log("商店：下一關按鈕被點擊。");

        // 這裡的邏輯需要與 GameProgressionManager 的場景流程圖保持一致
        // 根據你之前的流程圖：Level1 -> 商店 -> Story2 -> Level2 -> 商店 -> Story3
        // 這個邏輯假設商店是在 Level1 和 Level2 之後才進入。
        // GameProgressionManager.AdvanceLevel() 和 GameProgressionManager.AdvanceStory() 應放在正確的呼叫點
        // 如果商店結束後總是進入下一個劇情，則統一處理

        // 統一處理流程推進：先推進關卡和劇情，然後加載對應的下一個場景
        // 這需要根據你的實際遊戲流程來判斷是進入劇情還是下一關卡
        GameProgressionManager.AdvanceLevel(); // 推進關卡進度
        GameProgressionManager.AdvanceStory(); // 推進劇情進度

        // 假設商店結束後總是進入下一段劇情 (如 StoryScene2 或 StoryScene3)
        GameProgressionManager.LoadNextStoryScene();
        
        // 如果你的邏輯是：
        // Level1 -> 商店 -> Story2 -> Level2 -> 商店 -> Story3 -> Level3
        // 那你可能需要更精確的判斷，例如：
        // if (GameProgressionManager.CurrentLevelIndex == 1) // 從 Level1 進入商店
        // {
        //     GameProgressionManager.LoadNextStoryScene(); // 加載 StoryScene2
        // }
        // else if (GameProgressionManager.CurrentLevelIndex == 2) // 從 Level2 進入商店
        // {
        //     GameProgressionManager.LoadNextStoryScene(); // 加載 StoryScene3
        // }
        // else { /* 其他關卡的商店結束，例如加載下一關遊戲 GameProgressionManager.LoadNextGameScene(); */ }
    }

    void Buy()
    {
        // 假設購買一個血量上限的提升
        int purchaseCost = 3; // 購買所需金幣數
        int maxHealthIncreaseAmount = 1; // 每次購買增加的最大血量上限

        // 檢查金幣是否足夠 (假設 CoinManager 是單例且GetCoinCount/RemoveCoin方法存在)
        if (CoinManager.instance != null && CoinManager.instance.GetCoinCount() >= purchaseCost)
        {
            Debug.Log($"商店：嘗試購買最大血量，花費 {purchaseCost} 金幣。");
            CoinManager.instance.RemoveCoin(purchaseCost); // 從 CoinManager 移除金幣

            // 呼叫 GameProgressionManager 來增加玩家的最大血量上限
            // GameProgressionManager.PlayerMaxHealth 是屬性，直接讀取
            // SetPlayerMaxHealth 是我們之前為 GameProgressionManager 新增的方法
            GameProgressionManager.SetPlayerMaxHealth(GameProgressionManager.PlayerMaxHealth + maxHealthIncreaseAmount); // <<<< 修正 >>>>

            Debug.Log("商店：成功購買最大血量上限！");
            // UI 更新應由 ShopScript 訂閱 GameProgressionManager 的事件自動完成
        }
        else
        {
            Debug.Log("商店：金幣不足，無法購買最大血量！");
        }
    }
}