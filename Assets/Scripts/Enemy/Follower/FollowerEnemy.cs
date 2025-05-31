using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class FollowerEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 5f;
    public float jumpCooldown = 2f;
    private float jumpTimer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Control")]
    [SerializeField] private float horizontalFactor = 0.4f;


    private Transform player;
    private Rigidbody2D rb;

    private bool facingRight = false; // 預設圖片面朝左，邏輯面也視為朝左

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        jumpTimer = jumpCooldown;
    }

    void Update()
    {
        if (player == null) return;

        jumpTimer -= Time.deltaTime;

        if (jumpTimer <= 0f && IsGrounded())
        {
            JumpTowardsPlayer();
            jumpTimer = jumpCooldown;
        }
    }

    private void JumpTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        rb.linearVelocity = Vector2.zero;

        float horizontalFactor = 0.4f; // ⭐️ 調整這裡控制水平移動量
        Vector2 jumpDirection = new Vector2(direction.x * horizontalFactor, 1f).normalized;

        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);

        if (direction.x < 0 && facingRight)
            Flip();
        else if (direction.x > 0 && !facingRight)
            Flip();
    }


    private bool IsGrounded()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        return hit != null;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Health health = collision.collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(1);
            }
        }
    }
}
