using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))  
        {
            if (CoinManager.instance != null)
            {
                CoinManager.instance.AddCoin(coinValue); 
                Destroy(gameObject); 
            }
            else
            {
                Debug.LogError("<color=red>Coin Error:</color> 玩家觸碰到金幣但 CoinManager.instance 為 null！金幣未被計入，請檢查 CoinManager 的初始化。", this);
                Destroy(gameObject); 
            }
        }
    }
}