using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("攻擊設定")]
    public Animator animator; // 拖曳 Player 物件的 Animator 元件到這裡
    public Transform attackPoint; // 拖曳 Player 物件下的 AttackPoint 空物件到這裡 (攻擊偵測圓圈的中心)
    public float attackRange = 1f; // 攻擊偵測圓圈的半徑 (紅色圓圈)
    public int attackDamage = 1; // 每次攻擊造成的傷害值
    public LayerMask Enemy; // 拖曳 Boss 的 Layer 到這裡 (在 Inspector 中選擇 Boss 所屬的 Layer，例如 "Enemy")

    [Header("攻擊後回彈設定")]
    public PlayerMovement playerMovement; // 拖曳 Player 物件上的 PlayerMovement 腳本到這裡
    public float recoilDuration = 0.3f; // 攻擊命中 Boss 後，回彈到上一個靜止位置所需的時間

    void Update()
    {
        // 偵測空白鍵 (Spacebar) 是否被按下，作為攻擊輸入
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    void Attack()
    {
        // 判斷 Player 是否正在回彈中，如果在回彈則不能再次攻擊
        if (playerMovement != null && playerMovement.IsRecoilActive()) 
        {
            //Debug.Log("Player 正在回彈中，無法攻擊。");
            return; // 終止攻擊，不執行後續邏輯
        }

        // 觸發 Animator 中的 "Attack" Trigger，播放揮刀動畫
        animator.SetTrigger("Attack"); 

        // 進行圓形範圍碰撞偵測
        // Physics2D.OverlapCircleAll 會在 attackPoint.position 周圍畫一個圓，
        // 並找出所有在 attackRange 半徑內，且屬於 "Enemy" Layer 的 Collider2D 物件。
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, Enemy);

        // 遍歷所有被擊中的碰撞體
        foreach (Collider2D hitCollider in hits) 
        {
            Debug.Log("擊中物體名稱：" + hitCollider.name);

            // 嘗試從被擊中的物體上獲取 BossHealth 腳本
            // 由於 Player 目前只攻擊 Boss，所以直接嘗試獲取 BossHealth
            BossHealth bossHealthScript = hitCollider.GetComponent<BossHealth>(); 
            if (bossHealthScript != null)
            {
                // 如果找到 BossHealth 腳本，則調用其 TakeDamage 方法造成傷害
                bossHealthScript.TakeDamage(attackDamage); 
                //Debug.Log(hitCollider.name + " 的 BossHealth 腳本找到，並嘗試扣血。");

                // 如果成功擊中 Boss，並且 PlayerMovement 腳本存在，則啟動 Player 回彈
                if (playerMovement != null)
                {
                    playerMovement.StartRecoilToLastIdle(recoilDuration); // 呼叫回彈到上一個靜止位置的方法
                    //Debug.Log("Player 擊中 Boss，啟動回彈到上一個靜止位置。");
                }
            }
            else
            {
                // 如果擊中了屬於 Enemy Layer 的物件，但它沒有 BossHealth 腳本，則打印警告
                // 這可能發生在 Boss 召喚小兵，或者有其他非 Boss 但屬於 Enemy Layer 的物件被擊中時
                Debug.LogWarning(hitCollider.name + " 未找到 BossHealth 腳本！或該物件不是主要可攻擊的 Boss 目標。", hitCollider.gameObject);
            }
        }
    }

    // 在 Scene 視窗中可視化攻擊範圍 (當選中 Player 時顯示紅色圓圈)
    private void OnDrawGizmosSelected()
    {
        // 確保 attackPoint 物件已經設置
        if (attackPoint == null) return;

        Gizmos.color = Color.red; // 將繪製顏色設為紅色
        // 繪製一個空心圓來表示攻擊偵測範圍
        Gizmos.DrawWireSphere(attackPoint.position, attackRange); 
    }
}