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
    public float preDashWaitTime = 1f; // 新增：暴衝前等待時間
    public float postDashWaitTime = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private Transform currentTarget;

    private Vector2 pointAPos;
    private Vector2 pointBPos;

    private bool isWaiting = false;
    private bool isDashing = false;
    private bool isCoolingDown = false;
    private bool isPreparingToDash = false; // 新增：是否正在準備暴衝

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // 確保玩家物件有 "Player" 標籤
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (pointA != null) pointAPos = pointA.position;
        if (pointB != null) pointBPos = pointB.position;

        currentTarget = pointB;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 如果不在暴衝、冷卻或準備暴衝中，且玩家進入警戒範圍，則開始暴衝準備
        if (!isDashing && !isCoolingDown && !isPreparingToDash && distanceToPlayer <= alertRange)
        {
            StartCoroutine(DashTowardPlayer());
            return;
        }

        // 如果不在暴衝、冷卻、等待或準備暴衝中，則執行巡邏
        if (!isDashing && !isCoolingDown && !isWaiting && !isPreparingToDash)
        {
            Patrol();
        }

        // 更新動畫參數
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsDashing", isDashing);
        animator.SetBool("IsPreparingToDash", isPreparingToDash); // 更新準備暴衝動畫參數
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
        isPreparingToDash = true; // 進入準備暴衝狀態
        rb.linearVelocity = Vector2.zero; // 停止移動

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        Flip(direction);

        Debug.Log("🟡 怪物開始準備暴衝！");
        animator.SetFloat("Speed", 0); // 停止移動動畫，準備播放靜態的準備動畫

        // 播放等待畫面 (frog_chase)
        // animator.SetBool("IsPreparingToDash", true) 會在 FixedUpdate 中自動更新

        yield return new WaitForSeconds(preDashWaitTime); // 等待準備時間

        isPreparingToDash = false; // 退出準備暴衝狀態
        // animator.SetBool("IsPreparingToDash", false) 會在 FixedUpdate 中自動更新

        isDashing = true; // 進入暴衝狀態
        Debug.Log("💨 怪物向玩家方向暴衝！");

        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f); // 開始暴衝移動

        yield return new WaitForSeconds(dashDuration); // 暴衝持續時間

        rb.linearVelocity = Vector2.zero; // 停止移動

        Debug.Log("😮‍💨 暴衝結束，開始冷卻");

        isDashing = false; // 退出暴衝狀態
        StartCoroutine(PostDashCooldown());
    }

    IEnumerator PostDashCooldown()
    {
        isCoolingDown = true;
        animator.SetFloat("Speed", 0); // 停止動畫
        yield return new WaitForSeconds(postDashWaitTime);

        Debug.Log("🟢 冷卻結束，回到巡邏");
        // 判斷離哪個巡邏點近，設定為下一個目標
        currentTarget = (Vector2.Distance(transform.position, pointAPos) <
                         Vector2.Distance(transform.position, pointBPos)) ? pointA : pointB;
        // 根據目標方向翻轉怪物
        Flip((currentTarget.position.x - transform.position.x));
        isCoolingDown = false;
    }

    IEnumerator WaitThenSwitchTarget()
    {
        isWaiting = true;
        animator.SetFloat("Speed", 0); // 停止動畫
        yield return new WaitForSeconds(waitTime);
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        Flip((currentTarget.position.x - transform.position.x));
        isWaiting = false;
    }

    void Flip(float direction)
    {
        Vector3 scale = transform.localScale;
        // 根據方向翻轉 X 軸的縮放
        scale.x = (direction >= 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果碰撞到標籤為 "Player" 的物件
        if (collision.collider.CompareTag("Player"))
        {
            // 嘗試取得 Health 組件
            Health health = collision.collider.GetComponent<Health>();
            if (health != null)
            {
                // 對玩家造成 1 點傷害
                health.TakeDamage(1);
            }
        }
    }

    void OnDrawGizmos()
    {
        // 在編輯器中繪製巡邏點連線和球體
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }

#if UNITY_EDITOR
        // 在編輯器中繪製警戒範圍
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRange);
#endif
    }
}
