using UnityEngine;

public class PlayerAttack2 : MonoBehaviour
{
    [Header("攻擊設定")]
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 1f;
    public int attackDamage = 1;
    public LayerMask enemyLayer;

    [Header("攻擊後回彈設定")]
    public PlayerController playerMovement; 

    public float recoilDuration = 0.3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    void Attack()
    {
        if (playerMovement != null && playerMovement.IsRecoilActive())
            return;

        animator.SetTrigger("Attack");

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("擊中：" + hit.name);

            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"{hit.name} 受到 {attackDamage} 傷害");

                if (playerMovement != null)
                {
                    playerMovement.StartRecoilToLastIdle(recoilDuration); // ✅ 你需要確保這個方法存在於 PlayerMovement
                }
            }
            else
            {
                Debug.LogWarning($"{hit.name} 沒有 Health 腳本，無法造成傷害。", hit.gameObject);
            }
        }

        Debug.Log("Attack triggered");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
