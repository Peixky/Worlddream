using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 10f;

    [Header("Environment Detection")]
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private Animator anim;

    private float horizontalInput;
    public bool canMove = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!canMove) return;

        // 取得水平輸入
        horizontalInput = Input.GetAxis("Horizontal");

        // 控制水平移動
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        // 角色翻面
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // 動畫參數設置
        anim.SetBool("run", Mathf.Abs(horizontalInput) > 0.01f);
        anim.SetBool("grounded", isGrounded());

        // 跳躍（使用 W 或 ↑）
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded())
        {
            Jump();
        }
    }

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
        anim.SetTrigger("jump");
    }

    private bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            0.1f,
            groundLayer
        );
        return hit.collider != null;
    }

    public bool CanAttack()
    {
        return horizontalInput == 0 && isGrounded();
    }

    public void Bounce(Vector2 force)
    {
        body.linearVelocity = Vector2.zero;
        body.AddForce(force, ForceMode2D.Impulse);
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCollider.bounds.center + Vector3.down * 0.1f, boxCollider.bounds.size);
    }

        private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
        }
    }

}
