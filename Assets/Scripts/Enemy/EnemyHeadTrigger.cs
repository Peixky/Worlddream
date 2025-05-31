using UnityEngine;

public class EnemyHeadTrigger : MonoBehaviour
{
    public EnemyHealth enemyHealth; // 從 Inspector 指定

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("👟 玩家踩中怪物頭！");
            enemyHealth.TakeDamage(1);

            // 玩家反彈跳一下（選擇性）
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
            }
        }
    }
}
