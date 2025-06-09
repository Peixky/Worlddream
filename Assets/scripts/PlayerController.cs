using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float ziplineSpeed = 5f;
    [SerializeField] private float jumpOffDistance = 0.5f;

    [Header("Environment Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer; 

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator anim;

    private float horizontalInput;
    public bool canMove = true;
    private bool isRecoiling = false;

    // Zipline
    private Zipline currentZipline; 
    private bool onZipline = false;
    private float t = 0f;
    private Vector2 startPoint, endPoint, direction;
    private float ziplineLength;

    private int playerLayerIndex; 
    private int platformLayerIndex; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        playerLayerIndex = gameObject.layer; 
        platformLayerIndex = LayerMask.NameToLayer("Platform");

        if (platformLayerIndex == -1)
        {
            Debug.LogWarning("PlayerController: 未找到 'Platform' Layer！請確保已在 Physics2D Layer 設定中添加此 Layer。", this);
        }
    }

    private void Update()
    {
        if (!canMove || isRecoiling) return;

        if (onZipline)
        {
            if (t >= 1f || Input.GetKeyDown(KeyCode.Space))
            {
                JumpOffZipline();
            }
            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        anim.SetBool("run", Mathf.Abs(horizontalInput) > 0.01f);
        anim.SetBool("grounded", IsGrounded());

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && IsGrounded())
        {
            Jump();
        }
        
        // 滑鼠左鍵點擊 → 瞬移玩家到滑鼠位置
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f; 

            transform.position = mouseWorldPos;

            rb.linearVelocity = Vector2.zero;
        }

    }

    private void FixedUpdate()
    {
        if (onZipline)
        {
            float delta = (ziplineSpeed / ziplineLength) * Time.fixedDeltaTime;
            t = Mathf.Clamp01(t + delta);

            Vector2 pos = Vector2.Lerp(startPoint, endPoint, t);
            transform.position = pos;

            rb.linearVelocity = direction * ziplineSpeed;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        anim.SetTrigger("jump");
    }

    public bool IsGrounded()
    {
        LayerMask combinedGroundLayers = groundLayer | platformLayer;
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, combinedGroundLayers);
        return hit.collider != null;
    }

    public void Bounce(Vector2 force)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void StartKnockbackIgnoreCollision(float duration) 
    {
        if (isRecoiling) return; 
        StartCoroutine(RecoilRoutine(duration));
    }

    private IEnumerator RecoilRoutine(float duration) 
    {
        isRecoiling = true;
        canMove = false;
        anim.SetBool("run", false);
        anim.SetTrigger("recoil");

        // 在擊退時暫時忽略與 Platform Layer 的碰撞
        if (platformLayerIndex != -1 && playerLayerIndex != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayerIndex, platformLayerIndex, true);
            Debug.Log("PlayerController: 玩家擊退時暫時忽略 Platform Layer 碰撞。");
        }

        yield return new WaitForSeconds(duration);
        isRecoiling = false;
        canMove = true; 
        if (platformLayerIndex != -1 && playerLayerIndex != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayerIndex, platformLayerIndex, false);
            Debug.Log("PlayerController: 玩家擊退結束，恢復 Platform Layer 碰撞。");
        }
    }

    public void StartRecoilToLastIdle(float duration)
    {
        StartCoroutine(RecoilRoutine(duration));
    }

    public bool IsRecoilActive()
    {
        return isRecoiling;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Zipline zipline = other.GetComponent<Zipline>();
        if (zipline != null && !onZipline)
        {
            AttachToZipline(zipline);
        }
    }

    public void AttachToZipline(Zipline zipline)
    {
        currentZipline = zipline;
        onZipline = true;

        startPoint = zipline.startPoint.position;
        endPoint = zipline.endPoint.position;
        direction = (endPoint - startPoint).normalized;
        ziplineLength = Vector2.Distance(startPoint, endPoint);

        Vector2 closest = zipline.ClosestPoint(transform.position);
        float distFromStart = Vector2.Distance(startPoint, closest);
        t = distFromStart / ziplineLength;

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        transform.position = closest;
    }

    private void JumpOffZipline()
    {
        onZipline = false;
        rb.gravityScale = 1f;

        Vector2 dashOffset = direction * jumpOffDistance;
        transform.position += (Vector3)dashOffset;

        if (currentZipline != null)
        {
            StartCoroutine(ResetZiplineCollider(currentZipline));
        }

        currentZipline = null;
    }

    private IEnumerator ResetZiplineCollider(Zipline zipline)
    {
        Collider2D ziplineCollider = zipline.GetComponent<Collider2D>();
        if (ziplineCollider != null)
        {
            ziplineCollider.enabled = false;
            yield return new WaitForSeconds(0.5f);
            ziplineCollider.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = collision.transform;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            StartCoroutine(ClearParentAfterFrame());
        }
    }

    private IEnumerator ClearParentAfterFrame()
    {
        yield return null; 
        if (transform.parent != null && transform.parent.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
        }
    }
}