using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("跳躍設定")]
    public float jumpForce = 20f;
    public float jumpMinInterval = 2f;
    public float jumpMaxInterval = 4f;
    public CameraShake cameraShake;
    public float bounceForce = 5f;
    public float bounceRadius = 5f;
    public LayerMask platformLayer;
    public float platformDetectRadius = 3f;

    [Header("投擲設定")]
    public GameObject[] projectiles;
    public Transform throwPoint;
    public float throwForce = 10f;
    public float throwMinInterval = 2f;
    public float throwMaxInterval = 5f;

    private Rigidbody2D rb;
    private Transform player;
    private bool isPlayerDead = false;
    private bool isJumping = false;

    // ✅ 改用通用的 Health 腳本
    private Health bossHealthSystem;

    private void OnEnable()
    {
        GameEvents.OnPlayerDied += OnPlayerDied;
        GameEvents.OnGameStart += OnGameStart;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= OnPlayerDied;
        GameEvents.OnGameStart -= OnGameStart;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        bossHealthSystem = GetComponent<Health>(); // ✅ 取得通用 Health 腳本
    }

    private void Update()
    {
        if (IntroManager.currentGameState != IntroManager.GameState.Playing ||
            isPlayerDead ||
            bossHealthSystem == null || bossHealthSystem.IsDead) // ✅ 使用 IsDead 屬性
            return;
    }

    private void OnPlayerDied()
    {
        Debug.Log("Boss 偵測到玩家死亡，停止行為！");
        isPlayerDead = true;
        StopAllCoroutines();
        CancelInvoke();
    }

    private void OnGameStart()
    {
        Debug.Log("BossController 偵測到遊戲開始，啟動 Boss 行為！");
        if (!isPlayerDead && bossHealthSystem != null && !bossHealthSystem.IsDead)
        {
            StartCoroutine(JumpRoutine());
            StartCoroutine(ThrowRoutine());
        }
    }

    IEnumerator JumpRoutine()
    {
        while (!isPlayerDead && bossHealthSystem != null && !bossHealthSystem.IsDead)
        {
            float waitTime = Random.Range(jumpMinInterval, jumpMaxInterval);
            yield return new WaitForSeconds(waitTime);
            Jump();
        }
    }

    void Jump()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (bossHealthSystem != null && bossHealthSystem.IsDead) return;

        if (isJumping && collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            HandlePlatformShatter(collision.contacts[0].point);

            if (cameraShake != null)
                StartCoroutine(cameraShake.Shake(0.3f, 0.4f));

            BounceNearbyPlayers();
        }
    }

    void HandlePlatformShatter(Vector2 collisionPoint)
    {
        Collider2D[] hitPlatforms = Physics2D.OverlapCircleAll(collisionPoint, platformDetectRadius, platformLayer);

        foreach (Collider2D platformCol in hitPlatforms)
        {
            if (platformCol.CompareTag("DestructiblePlatform"))
            {
                DestructiblePlatform destructiblePlatform = platformCol.GetComponent<DestructiblePlatform>();
                if (destructiblePlatform != null)
                {
                    destructiblePlatform.StartShatter();
                }
            }
        }
    }

    void BounceNearbyPlayers()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, bounceRadius);

        foreach (Collider2D col in hits)
        {
            if (col.CompareTag("Player"))
            {
                Rigidbody2D playerRb = col.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
                }
            }
        }
    }

    IEnumerator ThrowRoutine()
    {
        while (!isPlayerDead && bossHealthSystem != null && !bossHealthSystem.IsDead)
        {
            float wait = Random.Range(throwMinInterval, throwMaxInterval);
            yield return new WaitForSeconds(wait);
            ThrowAtPlayer();
        }
    }

    void ThrowAtPlayer()
    {
        if (projectiles.Length == 0 || player == null) return;

        GameObject selected = projectiles[Random.Range(0, projectiles.Length)];
        GameObject proj = Instantiate(selected, throwPoint.position, Quaternion.identity);

        Vector2 dir = (player.position - throwPoint.position).normalized;
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * throwForce;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bounceRadius);

        if (GetComponent<Collider2D>() != null)
        {
            Gizmos.color = Color.blue;
            Vector2 gizmoCenter = (Vector2)transform.position - new Vector2(0, GetComponent<Collider2D>().bounds.extents.y);
            Gizmos.DrawWireSphere(gizmoCenter, platformDetectRadius);
        }
    }
}
