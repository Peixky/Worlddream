using UnityEngine;
using System.Collections;

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

    private bool isRecoiling = false; // ✅ 新增 recoil 狀態追蹤

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!canMove || isRecoiling) return; // ✅ 禁止在 recoil 狀態下移動

        horizontalInput = Input.GetAxis("Horizontal");

        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        anim.SetBool("run", Mathf.Abs(horizontalInput) > 0.01f);
        anim.SetBool("grounded", isGrounded());

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

    public bool isGrounded()
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

    // ✅ recoil 機制
    public void StartRecoilToLastIdle(float duration)
    {
        if (isRecoiling) return;
        StartCoroutine(RecoilRoutine(duration));
    }

    private IEnumerator RecoilRoutine(float duration)
    {
        isRecoiling = true;
        canMove = false;
        anim.SetBool("run", false);
        anim.SetTrigger("recoil"); // 可選，如果有 recoil 動畫
        yield return new WaitForSeconds(duration);
        isRecoiling = false;
        canMove = true;
    }

    public bool IsRecoilActive()
    {
        return isRecoiling;
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
