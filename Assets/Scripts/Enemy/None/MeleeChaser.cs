using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MeleeChaser : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    private float attackTimer;
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            // 追玩家
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // 停止移動 & 攻擊
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                player.GetComponent<Health>()?.TakeDamage(1);
                attackTimer = 0f;
            }
        }
    }
}
