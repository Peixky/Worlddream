using UnityEngine;

public class EnemyHeadTrigger : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public float damageCooldown = 0.5f;
    public float bounceForce = 12f;

    private float lastDamageTime = -999f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        float currentTime = Time.time;

        if (currentTime - lastDamageTime < damageCooldown)
        {
            Debug.Log("🛡️ 怪物無敵中，這次不扣血");
        }
        else
        {
            Debug.Log("👟 玩家踩中怪物頭！");
            enemyHealth.TakeDamage(1);
            lastDamageTime = currentTime;
        }

        // 無論是否成功造成傷害，都強制彈開
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
            // 強制給一個穩定向上的速度
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
        }
    }
}
