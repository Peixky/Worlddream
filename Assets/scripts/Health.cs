using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public int maxHealth = 3; // 最大血量
    public int currentHealth;

    [SerializeField] private PlayerHealthUI healthUI; // 拖入 PlayerHealthUI 腳本

    [SerializeField] private PlayerDeathManager deathManager; // 拖入 PlayerDeathManager 腳本

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth, maxHealth); // 初始化顯示愛心
        }
        else
        {
            Debug.LogWarning("healthUI 沒有設定！");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " 扣血！剩餘血量：" + currentHealth);

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " 死亡");

        if (deathManager != null)
        {
            deathManager.TriggerDeath();
        }
        else
        {
            Debug.LogWarning("deathManager 沒有設定！");
        }
    }
}
