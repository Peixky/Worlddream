using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1; // 每次觸碰到金幣時增加的金幣數量

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))  // 確認是否是玩家碰撞
        {
            CoinManager.instance.AddCoin(coinValue);  // 呼叫 CoinManager 增加金幣
            Destroy(gameObject);  // 銷毀金幣物件
        }
    }
}
