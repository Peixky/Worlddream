using UnityEngine;
using UnityEngine.SceneManagement; 

public class CoinManager : MonoBehaviour
{
    
    public static CoinManager instance { get; private set; }

    private int coinCount = 0; 

    private CoinUI currentCoinUI;

    private void Awake()
    {
    
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.LogWarning($"<color=orange>CoinManager:</color> 檢測到重複實例 ({gameObject.name})，銷毀自身以保持單例模式。");
            Destroy(gameObject);
            return;
        }
        
        Debug.Log($"<color=cyan>CoinManager:</color> Awake() 結束。金幣數量初始化為: {coinCount}.");
    }
    public void RegisterCoinUI(CoinUI ui)
    {
        if (ui != null)
        {
            currentCoinUI = ui;
            currentCoinUI.UpdateCoinText(coinCount); // 註冊後立即更新 UI 顯示為當前金幣數量
            Debug.Log($"<color=green>CoinManager:</color> CoinUI ({ui.gameObject.name}) 已成功註冊。");
        }
        else
        {
            Debug.LogWarning("<color=orange>CoinManager:</color> 嘗試註冊的 CoinUI 為 null。");
        }
    }

    public void AddCoin(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("<color=orange>CoinManager:</color> 嘗試添加負數金幣。請使用 RemoveCoin 方法。");
            return;
        }

        coinCount += amount;
        Debug.Log($"<color=blue>CoinManager:</color> 金幣增加 {amount}。當前金幣數量：{coinCount}");

        if (currentCoinUI != null)
        {
            currentCoinUI.UpdateCoinText(coinCount);
        }
        else
        {
            Debug.LogWarning("<color=orange>CoinManager:</color> CoinUI 尚未註冊，金幣數量未在 UI 上更新。");
        }


    }

    public void RemoveCoin(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("<color=orange>CoinManager:</color> 嘗試移除負數金幣。請使用 AddCoin 方法。");
            return;
        }

        coinCount -= amount;
        if (coinCount < 0) coinCount = 0; 

        Debug.Log($"<color=blue>CoinManager:</color> 金幣減少 {amount}。當前金幣數量：{coinCount}");

        if (currentCoinUI != null)
        {
            currentCoinUI.UpdateCoinText(coinCount);
        }
        else
        {
            Debug.LogWarning("<color=orange>CoinManager:</color> CoinUI 尚未註冊，金幣數量未在 UI 上更新。");
        }

    }

    public void ResetCoinCount()
    {
        coinCount = 0;
        Debug.Log("<color=blue>CoinManager:</color> 金幣數量已重置為 0。");
        if (currentCoinUI != null)
        {
            currentCoinUI.UpdateCoinText(coinCount);
        }
        // PlayerPrefs.SetInt("SavedCoinCount", coinCount);
        // PlayerPrefs.Save();
    }

    public int GetCoinCount()
    {
        return coinCount;
    }
}