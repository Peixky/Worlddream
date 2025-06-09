    using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public int healthPrice = 1; // 一滴血 1 元
    public Health playerHealth; 
    public Slider healthSlider; 

    void Start()
    {
        UpdateSliderUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetHealth(); // 測試重設用
        }
    }

    public void buyHealth()
    {
        if (playerHealth == null)
        {
            Debug.LogError("ShopScript: Player Health 未指定！");
            return;
        }

        int currentCoins = CoinManager.instance.GetCoinCount();

        if (currentCoins >= healthPrice)
        {
            CoinManager.instance.RemoveCoin(healthPrice);
            playerHealth.IncreaseMaxHealth(1);
            Debug.Log($"購買成功！+1 最大血量，剩餘金幣：{CoinManager.instance.GetCoinCount()}");
            UpdateSliderUI();
        }
        else
        {
            Debug.Log("金幣不足！");
        }
    }

    public void ResetHealth()
    {
        playerHealth.SetHealth(1); // 重設為 1 滴血
        Debug.Log("血量已重設為 1");
        UpdateSliderUI();
    }

    private void UpdateSliderUI()
    {
        if (playerHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = playerHealth.MaxHealth;
            healthSlider.value = playerHealth.CurrentHealth;
        }
    }
}
