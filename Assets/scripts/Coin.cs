using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1; // 每次觸碰到金幣時增加的金幣數量

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))  // 確認是否是玩家碰撞
        {
            // 在呼叫之前檢查 CoinManager.instance 是否為 null
            if (CoinManager.instance != null)
            {
                CoinManager.instance.AddCoin(coinValue);  // 呼叫 CoinManager 增加金幣
                Destroy(gameObject);  // 銷毀金幣物件
            }
            else
            {
                Debug.LogError("<color=red>Coin Error:</color> 玩家觸碰到金幣但 CoinManager.instance 為 null！金幣未被計入，請檢查 CoinManager 的初始化。", this);
                Destroy(gameObject); // 即使出錯也銷毀金幣，避免玩家持續遇到
            }
        }
    }
}