using UnityEngine;
using System.Collections; // 為了 StartCoroutine, 如果 PlayerController 裡面有 Recoil 是協程

public class PlayerAttack : MonoBehaviour
{
    [Header("攻擊設定")]
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 1f;
    public int attackDamage = 1; // 玩家目前的基礎攻擊力
    public LayerMask enemyLayer;
    public LayerMask destructibleWallLayer; // <<<< 新增：可破壞牆壁的 Layer >>>>

    [Header("攻擊後回彈設定")]
    public PlayerController playerMovement; 

    public float recoilDuration = 0.3f;

    void Update()
    {
        if (GameProgressionManager.currentGameState != GameProgressionManager.GameState.Playing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    void Attack()
    {
        // 玩家在回彈狀態時不能攻擊，這個檢查保持不變
        if (playerMovement != null && playerMovement.IsRecoilActive())
            return;

        animator.SetTrigger("Attack");

        LayerMask combinedAttackLayer = enemyLayer | destructibleWallLayer;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, combinedAttackLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("擊中：" + hit.name);

            DestructibleWall wall = hit.GetComponent<DestructibleWall>();
            if (wall != null)
            {
                wall.TakeDamage(attackDamage);
                Debug.Log($"{hit.name} 受到 {attackDamage} 傷害 (牆壁)");
                continue; 
            }

            Health enemyHealth = hit.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"{hit.name} 受到 {attackDamage} 傷害 (敵人)");

                // === 修正這裡：移除呼叫 StartRecoilToLastIdle ===
                // Recoil (回彈) 和 canMove (移動控制) 現在由 PlayerDamageHandler 負責觸發
                // PlayerDamageHandler 會呼叫 PlayerController.StartKnockbackIgnoreCollision
                // 而 StartKnockbackIgnoreCollision 內部包含了 RecoilRoutine 的啟動和 canMove 的管理。
                // 所以，PlayerAttack 不再需要在這裡直接控制回彈了。
                // if (playerMovement != null)
                // {
                //     playerMovement.StartRecoilToLastIdle(recoilDuration);
                // }
                // ===========================================
            }
            else
            {
                Debug.LogWarning($"{hit.name} 沒有 Health 或 DestructibleWall 腳本，無法造成傷害。", hit.gameObject);
            }
        }

        Debug.Log("Attack triggered");
    }

    public void IncreaseAttackPower(int amount)
    {
        attackDamage += amount;
        Debug.Log($"玩家攻擊力提升了 {amount}，當前攻擊力為 {attackDamage}");
    }

    public int GetCurrentAttackPower()
    {
        return attackDamage;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}