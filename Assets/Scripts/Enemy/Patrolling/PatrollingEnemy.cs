using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PatrollingEnemy : MonoBehaviour
{
    [Header("巡邏設定")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float waitTime = 1f;

    [Header("暴衝設定")]
    public float alertRange = 5f;
    public float dashSpeed = 6f;
    public float dashDistance = 5f;
    public float preDashWaitTime = 1f;
    public float postDashWaitTime = 2f;

    [Header("擊退效果")]
    public float knockbackForce = 500f;

    [Header("回到巡邏設定")]
    public float maxStayDuration = 5f; // 超過這個秒數就回到巡邏
    private float stayTime = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private Transform currentTarget;

    private Vector2 pointAPos;
    private Vector2 pointBPos;

    private Vector2 dashDirection = Vector2.zero;

    private bool isWaiting = false;
    private bool isDashing = false;
    private bool isCoolingDown = false;
    private bool isPreparingToDash = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (pointA != null) pointAPos = pointA.position;
        if (pointB != null) pointBPos = pointB.position;

        currentTarget = pointB;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= alertRange)
        {
            stayTime += Time.fixedDeltaTime;

            if (!isDashing && !isCoolingDown && !isPreparingToDash && stayTime <= maxStayDuration)
            {
                StartCoroutine(DashTowardPlayer());
                return;
            }

            if (stayTime > maxStayDuration)
            {
                Debug.Log("🔁 停留太久，返回巡邏");
                currentTarget = (Vector2.Distance(transform.position, pointAPos) <
                                 Vector2.Distance(transform.position, pointBPos)) ? pointA : pointB;
                Flip((currentTarget.position.x - transform.position.x));
                stayTime = 0f;
            }
        }
        else
        {
            stayTime = 0f; // 離開範圍就重置計時
        }

        if (!isDashing && !isCoolingDown && !isWaiting && !isPreparingToDash)
        {
            Patrol();
        }

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsDashing", isDashing);
        animator.SetBool("IsPreparingToDash", isPreparingToDash);
    }

    void Patrol()
    {
        if (currentTarget == null || rb == null) return;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = (currentTarget == pointA) ? pointAPos : pointBPos;
        float xDistance = Mathf.Abs(currentPos.x - targetPos.x);

        if (xDistance < 0.1f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            StartCoroutine(WaitThenSwitchTarget());
            return;
        }

        float direction = Mathf.Sign(targetPos.x - currentPos.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        Flip(direction);
    }

    IEnumerator DashTowardPlayer()
    {
        isPreparingToDash = true;
        rb.linearVelocity = Vector2.zero;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        Flip(direction);

        animator.SetFloat("Speed", 0);
        Debug.Log("🟡 準備暴衝...");

        yield return new WaitForSeconds(preDashWaitTime);

        isPreparingToDash = false;
        isDashing = true;

        Vector2 startPos = rb.position;
        dashDirection = new Vector2(direction, 0f);
        Debug.Log("💨 暴衝中...");

        while (Vector2.Distance(startPos, rb.position) < dashDistance)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isDashing = false;

        Debug.Log("😮‍💨 暴衝結束");
        StartCoroutine(PostDashCooldown());
    }

    IEnumerator PostDashCooldown()
    {
        isCoolingDown = true;
        animator.SetFloat("Speed", 0);
        yield return new WaitForSeconds(postDashWaitTime);

        Debug.Log("🟢 冷卻結束，回到巡邏");

        currentTarget = (Vector2.Distance(transform.position, pointAPos) <
                         Vector2.Distance(transform.position, pointBPos)) ? pointA : pointB;

        Flip((currentTarget.position.x - transform.position.x));
        stayTime = 0f;
        isCoolingDown = false;
    }

    IEnumerator WaitThenSwitchTarget()
    {
        isWaiting = true;
        animator.SetFloat("Speed", 0);
        yield return new WaitForSeconds(waitTime);
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        Flip((currentTarget.position.x - transform.position.x));
        isWaiting = false;
    }

    void Flip(float direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = (direction >= 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Health health = collision.collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(1);
            }

            if (isDashing)
            {
                Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockbackDir = dashDirection.normalized;
                    Debug.Log("💥 擊退方向：" + knockbackDir);
                    Debug.Log("💥 施加擊退力");

                    playerRb.linearVelocity = knockbackDir * 10f;
                    // playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }

#if UNITY_EDITOR
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRange);
#endif
    }
}
