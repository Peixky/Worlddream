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

    private float wallJumpCooldown;
    private float horizontalInput;
    public bool canMove = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        canMove = true;
    }

    private void Update()
    {
        if (!canMove) return;

        horizontalInput = Input.GetAxis("Horizontal");

        // Flip sprite
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // Animation parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        if (wallJumpCooldown > 0.2f)
        {
            // Apply horizontal movement
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            // Wall slide
            if (onWall() && !isGrounded())
            {
                body.gravityScale = 1f;

                // Optional: Limit sliding speed
                if (body.linearVelocity.y < -2f)
                    body.linearVelocity = new Vector2(body.linearVelocity.x, -2f);

                anim.SetBool("onWall", true); // Optional for wall slide animation
            }
            else
            {
                body.gravityScale = 3f;
                anim.SetBool("onWall", false);
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (onWall() && !isGrounded())
                {
                    WallJump(); // 優先跳牆
                }
                else if (isGrounded())
                {
                    Jump(); // 不在牆上才跳地板
                }
            }

        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
        anim.SetTrigger("jump");
    }

    private void WallJump()
    {
        float jumpDirectionX = -Mathf.Sign(transform.localScale.x); // 往反方向跳
        float wallJumpX = (horizontalInput == 0) ? 10f : 3f;
        float wallJumpY = (horizontalInput == 0) ? 6f : 6f;

        // 重設速度，避免與下墜疊加
        body.linearVelocity = Vector2.zero;

        body.linearVelocity = new Vector2(jumpDirectionX * wallJumpX, wallJumpY);

        // 讓角色面對新方向
        transform.localScale = new Vector3(jumpDirectionX, transform.localScale.y, transform.localScale.z);

        wallJumpCooldown = 0f;

        anim.SetTrigger("jump");
    }


    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            0.1f,
            groundLayer
        );

        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        Vector2 direction = new Vector2(transform.localScale.x, 0);
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            direction,
            0.1f,
            groundLayer
        );

        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }

    private void EnableMovement()
    {
        canMove = true;
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

        Gizmos.color = Color.red;
        Vector3 direction = new Vector3(transform.localScale.x, 0, 0);
        Gizmos.DrawWireCube(boxCollider.bounds.center + direction * 0.1f, boxCollider.bounds.size);
    }
}
