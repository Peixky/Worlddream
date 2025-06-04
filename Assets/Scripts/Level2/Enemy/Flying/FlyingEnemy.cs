using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyingEnemy : MonoBehaviour
{
    [Header("巡邏設定")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float pauseTime = 1f;

    [Header("炸彈攻擊")]
    public GameObject bombPrefab;
    public float attackRange = 6f;
    public float bombInterval = 2f;

    private Transform currentTarget;
    private Transform player;
    private float bombTimer;

    private Rigidbody2D rb;
    private bool isWaiting = false;

    private Vector2 pointAPos;
    private Vector2 pointBPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (pointA != null) pointAPos = pointA.position;
        if (pointB != null) pointBPos = pointB.position;

        currentTarget = pointB;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Patrol();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            bombTimer += Time.fixedDeltaTime;
            if (bombTimer >= bombInterval)
            {
                ThrowBomb();
                bombTimer = 0f;
            }
        }
    }

    void Patrol()
    {
        if (currentTarget == null || isWaiting) return;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = (currentTarget == pointA) ? pointAPos : pointBPos;
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            if (!isWaiting)
            {
                isWaiting = true;
                StartCoroutine(WaitBeforeSwitchTarget());
            }
            return;
        }

        float direction = Mathf.Sign(targetPos.x - currentPos.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, 0f);

        // ✅ 即時翻轉（圖片預設朝左）
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * -direction;
        transform.localScale = scale;
    }

    IEnumerator WaitBeforeSwitchTarget()
    {
        yield return new WaitForSeconds(pauseTime);
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        isWaiting = false;
    }

    void ThrowBomb()
    {
        if (bombPrefab == null || player == null) return;

        GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);

        Vector2 direction = (player.position - transform.position).normalized;
        float bombSpeed = 7f;

        Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
        if (bombRb != null)
        {
            bombRb.linearVelocity = direction * bombSpeed;
        }

        bomb.transform.right = -direction;
    }


    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
