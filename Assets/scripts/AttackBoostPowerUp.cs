using UnityEngine;

public class AttackBoostPowerUp : MonoBehaviour
{
    [Header("攻擊力提升設定")]
    public int attackPowerIncrease = 5; // 玩家攻擊力增加量
    public float sizeMultiplier = 1.2f; // 玩家尺寸的乘數 (例如 1.2f 放大 20%)

    [Header("音效與視覺特效 (可選)")]
    public GameObject pickUpEffectPrefab; // 撿到時播放的特效 Prefab
    public AudioClip pickUpSound; // 撿到時播放的音效 AudioClip

    private AudioSource audioSource; 

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // === 修正這裡：直接呼叫 PlayerAttack 腳本上的方法 ===
            PlayerAttack playerAttackScript = other.GetComponent<PlayerAttack>(); 
            if (playerAttackScript != null)
            {
                playerAttackScript.IncreaseAttackPower(attackPowerIncrease); // 增加攻擊力
                Debug.Log($"玩家攻擊力增加 {attackPowerIncrease}！當前攻擊力：{playerAttackScript.GetCurrentAttackPower()}");
            }
            else
            {
                Debug.LogWarning("PlayerAttack 腳本未找到於玩家物件上！無法增加攻擊力。", other.gameObject);
            }
            // ===============================================

            // --- 放大玩家尺寸 ---
            other.transform.localScale *= sizeMultiplier;
            Debug.Log($"玩家尺寸放大 {sizeMultiplier} 倍！新尺寸：{other.transform.localScale}");

            // --- 播放撿到音效與特效 ---
            if (pickUpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickUpSound);
            }
            else if (pickUpSound == null)
            {
                Debug.LogWarning("AttackBoostPowerUp: 撿取音效未設定！", this);
            }
            else if (audioSource == null)
            {
                 Debug.LogWarning("AttackBoostPowerUp: AudioSource 組件遺失於 PowerUp 物件上，無法播放音效！", this);
            }

            if (pickUpEffectPrefab != null)
            {
                Instantiate(pickUpEffectPrefab, transform.position, Quaternion.identity);
            }

            // --- 銷毀自身 ---
            Destroy(gameObject); 
        }
    }
}