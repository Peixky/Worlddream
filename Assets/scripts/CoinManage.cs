
using UnityEngine;  // 必須加入這行
public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;
    private int coinCount = 0;

    public CoinUI coinUI; // 在 Inspector 連結 CoinUI 物件

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        Debug.Log("金幣數量：" + coinCount);
        coinUI.UpdateCoinText(coinCount);
    }
}
