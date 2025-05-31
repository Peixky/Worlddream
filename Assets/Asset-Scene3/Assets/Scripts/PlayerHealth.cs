using UnityEngine;
using System; 

public class PlayerHealth : MonoBehaviour
{
    [Header("血量設定")]
    public int maxHealth = 9; 
    private int currentHealth; 
    private bool isDead = false; 

    // <<<< 移除：OnHealthChanged 事件，因為 HealthBarUI 不存在了 >>>>
    // public event Action<int, int> OnHealthChanged; 
    
    public event Action OnPlayerDied; 

    [Header("Player 死亡設定")]
    public Animator playerAnimator; 
    public string playerDeathTrigger = "isDead"; 
    public PlayerMovement playerMovementScript; 
    
    public IntroManager introManager; 

    void Awake()
    {
        currentHealth = maxHealth; 

        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }
        if (playerMovementScript == null)
        {
            playerMovementScript = GetComponent<PlayerMovement>();
        }
        if (introManager == null) 
        {
            introManager = FindFirstObjectByType<IntroManager>(); 
        }

        // <<<< 移除：Awake 時的 OnHealthChanged 觸發 >>>>
        // OnHealthChanged?.Invoke(currentHealth, maxHealth);
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
        OnPlayerDied?.Invoke(); 

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(playerDeathTrigger); 
        }
        else
        {
            Debug.LogWarning("PlayerHealth: 未連結 Player Animator 或死亡 Trigger 未設定！無法播放死亡動畫。", this);
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false; 
        }
        else
        {
            Debug.LogWarning("PlayerHealth: 未連結 Player Movement 腳本！無法禁用玩家移動。", this);
        }
        
        if (introManager != null) 
        {
            IntroManager.ShowGameOver(); 
        }
        else
        {
            Debug.LogError("PlayerHealth: 未找到 IntroManager！無法顯示遊戲結束畫面。請確保場景中有 GameManager 物件且掛載 IntroManager 腳本。", this);
        }
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