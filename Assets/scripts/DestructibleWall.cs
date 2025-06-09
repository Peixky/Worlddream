using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [Header("牆壁生命值設定")]
    public int maxHealth = 30; 
    private int currentHealth;

    [Header("破壞視覺/音效 (可選)")]
    public GameObject destructionEffectPrefab; 
    public AudioClip destructionSound; 

    private SpriteRenderer spriteRenderer; 
    private AudioSource audioSource; 

    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && destructionSound != null)
        {
            Debug.LogWarning("DestructibleWall: AudioSource 組件遺失於牆壁 GameObject 上，無法播放破壞音效！", this);
        }
    }

    // 受到傷害的方法
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return; 

        currentHealth -= amount;
        Debug.Log($"牆壁受到 {amount} 傷害，剩餘血量：{currentHealth}");

        if (currentHealth <= 0)
        {
            DestroyWall();
        }
    }

    void DestroyWall()
    {
        Debug.Log("牆壁已被破壞！");

       
        if (destructionSound != null && audioSource != null)
        { 
            audioSource.PlayOneShot(destructionSound);
        }

        // 播放破壞特效
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 銷毀牆壁 GameObject
        Destroy(gameObject); 
    }
}