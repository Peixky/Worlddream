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
        if (CoinManager.instance != null)
        {
            CoinManager.instance.RegisterCoinUI(this);
        }
        else
        {
            Debug.LogWarning("<color=orange>CoinUI:</color> CoinManager 尚未初始化。金幣 UI 可能無法即時更新。");
            UpdateCoinText(0); // 先顯示 0
        }
    }

    public void UpdateCoinText(int amount)
    {
        coinText.text = "x " + amount;
    }
}