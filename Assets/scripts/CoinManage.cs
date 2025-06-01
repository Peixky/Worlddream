using UnityEngine;
using UnityEngine.SceneManagement; // 用於 Debug.Log 中顯示當前場景名稱

public class CoinManager : MonoBehaviour
{
    // 單例實例：public 讀取，private set 寫入，確保只有 CoinManager 自己能設定它
    public static CoinManager instance { get; private set; }

    private int coinCount = 0; // 當前金幣數量

    // 引用當前活躍的 CoinUI 腳本，由 CoinUI 自己註冊
    private CoinUI currentCoinUI;

    // ============== 單例模式初始化 (Awake) ==============
    private void Awake()
    {
        // 偵測 Awake 函數被呼叫的時機和所在的 GameObject
        Debug.Log($"<color=cyan>CoinManager:</color> Awake() 開始執行。當前場景: {SceneManager.GetActiveScene().name}. GameObject: {gameObject.name}.");

        // 檢查是否已經有實例存在
        if (instance == null)
        {
            instance = this; // 設定當前這個物件的實例為唯一的單例
            DontDestroyOnLoad(gameObject); // 確保這個 GameObject 在場景切換時不會被銷毀
            Debug.Log("<color=cyan>CoinManager:</color> 單例實例已成功建立並設定為 DontDestroyOnLoad。");
        }
        else if (instance != this)
        {
            // 如果已經存在另一個實例，且不是當前的這個實例，則銷毀當前的這個物件，以確保單例唯一性
            Debug.LogWarning($"<color=orange>CoinManager:</color> 檢測到重複實例 ({gameObject.name})，銷毀自身以保持單例模式。");
            Destroy(gameObject);
            return; // 銷毀後立即返回，不執行後續代碼
        }

        // 可以選擇在這裡加載之前保存的金幣數量 (如果遊戲需要保存金幣進度)
        // coinCount = PlayerPrefs.GetInt("SavedCoinCount", 0); 
        
        Debug.Log($"<color=cyan>CoinManager:</color> Awake() 結束。金幣數量初始化為: {coinCount}.");
    }

    // ============== 公共方法 (由其他腳本呼叫) ==============

    /// <summary>
    /// 讓 CoinUI 腳本在它自己的 Awake() 中呼叫此方法來註冊自己。
    /// </summary>
    /// <param name="ui">要註冊的 CoinUI 實例。</param>
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

    /// <summary>
    /// 添加金幣數量。
    /// </summary>
    /// <param name="amount">要添加的金幣數量。</param>
    public void AddCoin(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("<color=orange>CoinManager:</color> 嘗試添加負數金幣。請使用 RemoveCoin 方法。");
            return;
        }

        coinCount += amount;
        Debug.Log($"<color=blue>CoinManager:</color> 金幣增加 {amount}。當前金幣數量：{coinCount}");

        // 如果有註冊的 CoinUI，則更新其顯示
        if (currentCoinUI != null)
        {
            currentCoinUI.UpdateCoinText(coinCount);
        }
        else
        {
            Debug.LogWarning("<color=orange>CoinManager:</color> CoinUI 尚未註冊，金幣數量未在 UI 上更新。");
        }

        // 如果需要保存金幣數量，可以在這裡保存
        // PlayerPrefs.SetInt("SavedCoinCount", coinCount);
        // PlayerPrefs.Save();
    }

    /// <summary>
    /// 移除金幣數量。
    /// </summary>
    /// <param name="amount">要移除的金幣數量。</param>
    public void RemoveCoin(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("<color=orange>CoinManager:</color> 嘗試移除負數金幣。請使用 AddCoin 方法。");
            return;
        }

        coinCount -= amount;
        if (coinCount < 0) coinCount = 0; // 確保金幣數量不會小於 0

        Debug.Log($"<color=blue>CoinManager:</color> 金幣減少 {amount}。當前金幣數量：{coinCount}");

        if (currentCoinUI != null)
        {
            currentCoinUI.UpdateCoinText(coinCount);
        }
        else
        {
            Debug.LogWarning("<color=orange>CoinManager:</color> CoinUI 尚未註冊，金幣數量未在 UI 上更新。");
        }
        // PlayerPrefs.SetInt("SavedCoinCount", coinCount);
        // PlayerPrefs.Save();
    }

    /// <summary>
    /// 重置金幣數量為 0。
    /// </summary>
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

    /// <summary>
    /// 提供給外部腳本獲取當前金幣數量。
    /// </summary>
    /// <returns>當前金幣數量。</returns>
    public int GetCoinCount()
    {
        return coinCount;
    }
}