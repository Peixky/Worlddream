using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public float damageCooldown = 1f;

    private int currentHealth;
    private float lastDamageTime = -999f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        lastDamageTime = -999f; // 重置冷卻
    }

    public bool TakeDamage(int damage)
    {
        float currentTime = Time.time;

        if (currentTime - lastDamageTime < damageCooldown)
        {
            Debug.Log($"{gameObject.name} 🛡️ 正在無敵中，這次不扣血");
            return false;
        }

        currentHealth -= damage;
        lastDamageTime = currentTime;

        Debug.Log($"{gameObject.name} 受到傷害，剩下血量：{currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }

        return true; // 有成功扣血
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} 死亡！");
        Destroy(gameObject);
    }
}
