using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Health))]
public class BossController : MonoBehaviour
{
    [Header("跳躍設定")]
    public float jumpForce = 20f;
    public float jumpMinInterval = 2f;
    public float jumpMaxInterval = 4f;
    public CameraShake cameraShake;
    public float bounceForce = 5f;
    public float bounceRadius = 5f;

    [Header("地震波設定")]
    public GameObject shockwavePrefab;
    public Transform shockwaveSpawnPoint;

    [Header("投擲設定")]
    public GameObject[] projectiles;
    public Transform throwPoint;
    public float throwForce = 10f;
    public float throwMinInterval = 2f;
    public float throwMaxInterval = 5f;

    [Header("近距離擊退設定")]
    public GameObject knockbackObjectPrefab;
    public Transform knockbackThrowPoint; // ✅ 新增的獨立投射點
    public float knockbackRange = 3f;
    public float knockbackCooldown = 5f;

    private Rigidbody2D rb;
    private Transform player;
    private bool isPlayerDead = false;
    private bool isJumping = false;
    private bool canKnockback = true;

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
        bossHealthSystem = GetComponent<Health>();
    }

    private void Update()
    {
        if (IntroManager.currentGameState != IntroManager.GameState.Playing ||
            isPlayerDead ||
            bossHealthSystem == null || bossHealthSystem.IsDead)
            return;

        CheckForKnockbackAttack();
    }

    private void OnGameStart()
    {
        Debug.Log("✅ BossController 啟動成功！");

        StartCoroutine(JumpRoutine());
        StartCoroutine(ThrowRoutine());
    }


    private void OnPlayerDied()
    {
        Debug.Log("Boss 偵測到玩家死亡，停止行為！");
        isPlayerDead = true;
        StopAllCoroutines();
        CancelInvoke();
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

            if (cameraShake != null)
                StartCoroutine(cameraShake.Shake(0.3f, 0.4f));

            BounceNearbyPlayers();
            SpawnShockwave();

            Debug.Log("Boss 落地，產生地震波！");
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

    void SpawnShockwave()
    {
        if (shockwavePrefab != null && shockwaveSpawnPoint != null)
        {
            Vector3 offset = new Vector3(-0.5f, 0f, 0f);
            Instantiate(shockwavePrefab, shockwaveSpawnPoint.position + offset, Quaternion.identity);
        }
    }

    IEnumerator ThrowRoutine()
    {
        while (!isPlayerDead && bossHealthSystem != null && !bossHealthSystem.IsDead)
        {
            float healthPercent = bossHealthSystem.CurrentHealth / bossHealthSystem.MaxHealth;

            float min = throwMinInterval;
            float max = throwMaxInterval;

            if (healthPercent <= 0.3f)
            {
                min *= 0.4f;
                max *= 0.6f;
            }
            else if (healthPercent <= 0.5f)
            {
                min *= 0.6f;
                max *= 0.8f;
            }

            float wait = Random.Range(min, max);
            yield return new WaitForSeconds(wait);

            ThrowAtPlayer();
        }
    }

    void ThrowAtPlayer()
    {
        if (projectiles.Length == 0 || player == null || bossHealthSystem == null) return;

        float healthPercent = bossHealthSystem.CurrentHealth / bossHealthSystem.MaxHealth;

        int numberOfProjectiles = 1;
        if (healthPercent <= 0.3f)
            numberOfProjectiles = 3;
        else if (healthPercent <= 0.5f)
            numberOfProjectiles = 2;

        Vector2 baseDir = (player.position - throwPoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        float totalSpreadAngle = 30f;
        float angleStep = (numberOfProjectiles > 1) ? totalSpreadAngle / (numberOfProjectiles - 1) : 0f;
        float startAngle = baseAngle - totalSpreadAngle / 2f;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angle = startAngle + i * angleStep;
            float radian = angle * Mathf.Deg2Rad;
            Vector2 throwDir = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;

            GameObject selected = projectiles[Random.Range(0, projectiles.Length)];
            GameObject proj = Instantiate(selected, throwPoint.position, Quaternion.identity);

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = throwDir * throwForce;
            }
        }

        Debug.Log($"Boss 血量：{healthPercent:P0}，投擲 {numberOfProjectiles} 顆，方向扇形展開！");
    }

    // ===== 近距離擊退判定 =====
    void CheckForKnockbackAttack()
    {
        if (!canKnockback || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= knockbackRange)
        {
            KnockbackPlayer();
            StartCoroutine(KnockbackCooldownRoutine());
        }
    }

    void KnockbackPlayer()
    {
        if (knockbackObjectPrefab == null) return;

        Transform spawnPoint = (knockbackThrowPoint != null) ? knockbackThrowPoint : throwPoint;
        if (spawnPoint == null) return;

        GameObject obj = Instantiate(knockbackObjectPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.left * throwForce;
        }

        Debug.Log("Boss 發動近距離擊退攻擊！");
    }



    IEnumerator KnockbackCooldownRoutine()
    {
        canKnockback = false;
        yield return new WaitForSeconds(knockbackCooldown);
        canKnockback = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bounceRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, knockbackRange);
    }
}
