using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [Header("牆壁生命值設定")]
    public int maxHealth = 30; // 牆壁的總生命值（需要被攻擊 30 次）
    private int currentHealth;

    [Header("破壞視覺/音效 (可選)")]
    public GameObject destructionEffectPrefab; // 牆壁被破壞時的特效 Prefab
    public AudioClip destructionSound; // 牆壁被破壞時的音效

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

        // 可以添加視覺反饋，例如牆壁血量減少時變色或出現裂痕
        // Example: if (spriteRenderer != null) spriteRenderer.color = Color.Lerp(Color.red, Color.white, (float)currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            DestroyWall();
        }
    }

    void DestroyWall()
    {
        Debug.Log("牆壁已被破壞！");

        // 播放破壞音效
        if (destructionSound != null && audioSource != null)
        {
            // PlayOneShot 確保音效完整播放，即使 GameObject 立即銷毀
            audioSource.PlayOneShot(destructionSound);
            // 如果音效較長，確保 AudioSource 有足夠時間播放
            // 這裡可以延遲銷毀或在新的獨立 GameObject 上播放音效
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