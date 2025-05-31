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
    public float dashDuration = 0.5f;
    public float postDashWaitTime = 2f;

    private Rigidbody2D rb;
    private Transform player;
    private Transform currentTarget;

    private Vector2 pointAPos;
    private Vector2 pointBPos;

    private bool isWaiting = false;
    private bool isDashing = false;
    private bool isCoolingDown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (pointA != null) pointAPos = pointA.position;
        if (pointB != null) pointBPos = pointB.position;

        currentTarget = pointB;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (!isDashing && !isCoolingDown && distanceToPlayer <= alertRange)
        {
            StartCoroutine(DashTowardPlayer());
            return;
        }

        if (!isDashing && !isCoolingDown && !isWaiting)
        {
            Patrol();
        }
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

        // 翻面
        Flip(direction);
    }

    IEnumerator DashTowardPlayer()
    {
        isDashing = true;
        rb.linearVelocity = Vector2.zero;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        Flip(direction);

        Debug.Log("💨 怪物向玩家方向暴衝！");

        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;

        Debug.Log("😮‍💨 暴衝結束，開始冷卻");

        isDashing = false;
        StartCoroutine(PostDashCooldown());
    }

    IEnumerator PostDashCooldown()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(postDashWaitTime);

        Debug.Log("🟢 冷卻結束，回到巡邏");
        // 回到最近巡邏點
        currentTarget = (Vector2.Distance(transform.position, pointAPos) <
                         Vector2.Distance(transform.position, pointBPos)) ? pointA : pointB;
        Flip((currentTarget.position.x - transform.position.x));
        isCoolingDown = false;
    }

    IEnumerator WaitThenSwitchTarget()
    {
        isWaiting = true;
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
