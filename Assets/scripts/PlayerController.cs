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
    [SerializeField] private LayerMask platformLayer; // 新增：Platform Layer (拖曳進來)

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator anim;

    private float horizontalInput;
    public bool canMove = true;
    private bool isRecoiling = false;

    // Zipline
    private Zipline currentZipline; // 假設 Zipline 腳本存在
    private bool onZipline = false;
    private float t = 0f;
    private Vector2 startPoint, endPoint, direction;
    private float ziplineLength;

    private int playerLayerIndex; // 儲存玩家自身的 Layer 索引
    private int platformLayerIndex; // 儲存 Platform Layer 的索引

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        playerLayerIndex = gameObject.layer; // 獲取玩家自己的 Layer 索引
        platformLayerIndex = LayerMask.NameToLayer("Platform"); // 獲取 "Platform" Layer 的索引

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
        anim.SetBool("grounded", IsGrounded()); // IsGrounded 現在會檢查 Platform Layer

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && IsGrounded())
        {
            Jump();
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

    // 新增：用於在擊退時忽略碰撞
    public void StartKnockbackIgnoreCollision(float duration) 
    {
        if (isRecoiling) return; // 如果正在回彈中，則不重複觸發
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

        yield return new WaitForSeconds(duration); // 等待擊退持續時間

        isRecoiling = false;
        canMove = true; // 擊退結束後恢復移動

        // 擊退結束後恢復與 Platform Layer 的碰撞
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
            // 不再直接在這裡設定父物件，而是啟動一個協程來延遲執行
            StartCoroutine(ClearParentAfterFrame());
        }
        // 其他的 OnCollisionExit2D 邏輯...
    }

    // 新增的協程，用於延遲設定父物件
    private IEnumerator ClearParentAfterFrame()
    {
        // 等待一幀（或到幀的末尾），讓 Unity 完成當前的啟用/停用操作
        yield return null; 

        // 確保在設定前，玩家的父物件仍然是剛離開的平台，避免意外設定
        // 例如，如果玩家立刻跳到另一個平台上，這個檢查就很重要
        if (transform.parent != null && transform.parent.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
            // Debug.Log("玩家父物件已在下一幀設為 null。"); // 可選的調試訊息
        }
    }
}