using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public int maxHealth = 3;
    public int currentHealth;

    [SerializeField] private HealthBarUI healthBarUI;
    [SerializeField] private PlayerDeathManager deathManager;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogWarning("HealthBarUI 尚未連接！");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " 扣血！剩餘血量：" + currentHealth);

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
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
    }
}



