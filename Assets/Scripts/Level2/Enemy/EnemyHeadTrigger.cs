using UnityEngine;

public class EnemyHeadTrigger : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public float bounceForce = 12f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("👟 玩家踩中怪物頭！");

        bool damaged = enemyHealth.TakeDamage(1);

        if (!damaged)
        {
            Debug.Log("🛡️ 怪物無敵，這次沒受傷");
        }

        BouncePlayer(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 若玩家持續停留在怪物頭上，也強制彈開
        BouncePlayer(other);
    }

    void BouncePlayer(Collider2D player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
        }
    }
}
