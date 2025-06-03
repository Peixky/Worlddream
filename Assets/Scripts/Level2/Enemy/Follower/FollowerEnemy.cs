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
    [SerializeField] private Transform[] groundCheckPoints; // 支援多個地板檢查點
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Control")]
    [SerializeField] private float horizontalFactor = 0.4f;

    private Transform player;
    private Rigidbody2D rb;
    private bool facingRight = false;

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

        Vector2 jumpDirection = new Vector2(direction.x * horizontalFactor, 1f).normalized;
        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);

        if (direction.x < 0 && facingRight)
            Flip();
        else if (direction.x > 0 && !facingRight)
            Flip();
    }

    private bool IsGrounded()
    {
        foreach (Transform point in groundCheckPoints)
        {
            Collider2D hit = Physics2D.OverlapCircle(point.position, groundCheckRadius, groundLayer);
            if (hit != null)
                return true;
        }
        return false;
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
        if (groundCheckPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform point in groundCheckPoints)
            {
                if (point != null)
                    Gizmos.DrawWireSphere(point.position, groundCheckRadius);
            }
        }
    }
}
