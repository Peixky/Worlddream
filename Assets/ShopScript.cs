using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public Slider healthSlider;
    public int maxHealth = 10;
    int currentHealth;

    void Start()
    {
        SetDefs();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetHealth(); // ✅ 重設邏輯封裝
        }
    }

    void SetDefs()
    {
        currentHealth = PlayerPrefs.GetInt("health", 0);
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void buyHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += 1;
            PlayerPrefs.SetInt("health", currentHealth);
            healthSlider.value = currentHealth;
            Debug.Log("Health Upgraded");
        }
        else
        {
            Debug.Log("Health is full");
        }
    }

    public void ResetHealth()
    {
        PlayerPrefs.DeleteKey("health");
        currentHealth = 0;
        SetDefs();
        Debug.Log("Health reset");
    }
}
