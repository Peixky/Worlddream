using UnityEngine;
using System; 
using System.Collections; 

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 9; 
    private int currentHealth; 
    private bool isDead = false; 

    // <<<< 移除：OnHealthChanged 事件，因為 HealthBarUI 不存在了 >>>>
    // public event Action<int, int> OnHealthChanged; 
    
    public event Action OnBossDied; 

    [Header("Boss 死亡設定")]
    public float rotationSpeed = 100f; 
    public float targetRotationZ = -90f; 
    public float destroyDelay = 3f; 

    private Rigidbody2D rb; 
    private BossController bossController; 

    void Awake()
    {
        currentHealth = maxHealth; 
        rb = GetComponent<Rigidbody2D>();
        bossController = GetComponent<BossController>(); 
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; 

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); 

        Debug.Log(gameObject.name + " 受到 " + damage + " 點傷害，剩餘生命值：" + currentHealth);

        // <<<< 移除：OnHealthChanged 事件觸發 >>>>
        // OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die(); 
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " 死亡了！");
        OnBossDied?.Invoke(); 

        Collider2D bossCollider = GetComponent<Collider2D>();
        if (bossCollider != null)
        {
            bossCollider.enabled = false;
        }

        if (bossController != null)
        {
            bossController.enabled = false; 
            bossController.StopAllCoroutines(); 
            Debug.Log(gameObject.name + " 的 BossController 腳本已禁用，停止所有行為！");
        }
        else
        {
            Debug.LogWarning("BossHealth: 未找到 BossController 腳本！確保已掛載到 Boss 物件上。", this);
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;       
            rb.angularVelocity = 0f;          
            rb.gravityScale = 0;              
            rb.constraints = RigidbodyConstraints2D.FreezeAll; 
        }
        else
        {
    #if UNITY_EDITOR 
                Debug.LogWarning("BossHealth: Boss 沒有 Rigidbody2D 元件，無法執行物理倒下和凍結！", this);
    #endif
        }

        StartCoroutine(RotateToTarget());

        Destroy(gameObject, destroyDelay); 
    }

    IEnumerator RotateToTarget()
    {
        float currentRotationZ = transform.rotation.eulerAngles.z; 
        float startRotationZ = currentRotationZ; 
        float endRotationZ = targetRotationZ; 

        if (endRotationZ > startRotationZ && (endRotationZ - startRotationZ) > 180f)
        {
            startRotationZ -= 360f; 
        }
        else if (startRotationZ > endRotationZ && (startRotationZ - endRotationZ) > 180f)
        {
            endRotationZ -= 360f; 
        }

        float elapsedTime = 0f;
        float totalRotationDegrees = Mathf.Abs(endRotationZ - startRotationZ);
        float totalDuration = (rotationSpeed > 0) ? (totalRotationDegrees / rotationSpeed) : 0; 

        while (elapsedTime < totalDuration)
        {
            currentRotationZ = Mathf.Lerp(startRotationZ, endRotationZ, elapsedTime / totalDuration);
            transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);

            elapsedTime += Time.deltaTime; 
            yield return null; 
        }

        transform.rotation = Quaternion.Euler(0, 0, targetRotationZ); 
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); 
        // <<<< 移除：OnHealthChanged 事件觸發 >>>>
        // OnHealthChanged?.Invoke(currentHealth, maxHealth); 
        Debug.Log(gameObject.name + " 恢復了 " + amount + " 點血量，當前生命值：" + currentHealth);
    }
}