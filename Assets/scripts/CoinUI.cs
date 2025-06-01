using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    void Awake()
    {
        if (coinText == null)
        {
            Debug.LogError("CoinUI: coinText 未連結！請在 Inspector 中設定。", this);
            enabled = false;
            return;
        }

        // 在 Awake 時，將自身註冊到 CoinManager
        // 由於 CoinManager 的 Script Execution Order 應該更高，所以它應該已經存在。
        if (CoinManager.instance != null)
        {
            CoinManager.instance.RegisterCoinUI(this);
            // Debug.Log($"<color=green>CoinUI:</color> 成功向 CoinManager 註冊。");
        }
        else
        {
            // 如果 CoinManager 真的還沒初始化，這可能是個嚴重問題，但 CoinUI 還是會顯示 0
            Debug.LogWarning("<color=orange>CoinUI:</color> CoinManager 尚未初始化。金幣 UI 可能無法即時更新。");
            UpdateCoinText(0); // 先顯示 0
        }
    }

    public void UpdateCoinText(int amount)
    {
        coinText.text = "x " + amount;
        // Debug.Log($"<color=magenta>CoinUI:</color> 金幣文本文本更新為: x {amount}");
    }
}