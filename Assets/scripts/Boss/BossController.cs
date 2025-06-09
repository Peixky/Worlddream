using UnityEngine;
using System.Collections;
using System;

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
    public float shockwaveYOffset = -0.5f;

    [Header("投擲設定")]
    public GameObject[] projectiles;
    public Transform throwPoint;
    public float throwForce = 10f;
    public float throwMinInterval = 2f;
    public float throwMaxInterval = 5f;

    [Header("近距離擊退設定")]
    public GameObject knockbackObjectPrefab;
    public Transform knockbackThrowPoint;
    public float knockbackRange = 3f;
    public float knockbackCooldown = 5f;
    public float knockbackLaunchForce = 10f;

    // === Boss 血量階段與位移設定 ===
    [Header("Boss 血量階段設定")]
    public float phase2HealthThreshold = 0.5f; // 血量達到 50% (0.5) 時進入第二階段
    // Removed: public float phase2MoveDuration = 2f;
    // Removed: public float phase2JumpForce = 25f;
    // Removed: public Vector2 phase2TargetPosition = new Vector2(0f, 0f);
    public float phase2JumpFrequencyMultiplier = 0.5f; // 第二階段跳躍頻率乘數 (例如 0.5 表示頻率加倍)
    public float phase2ThrowFrequencyMultiplier = 0.5f; // 第二階段投擲頻率乘數
    
    private bool isInPhase2 = false;
    // Removed: private bool isPhaseTransitioning = false; // 不再有複雜的階段位移，所以不需要這個 flag
    // ====================================

    private Rigidbody2D rb;
    private Transform player;
    private bool isPlayerDead = false;
    private bool isJumping = false;
    private bool canKnockback = true;

    private Health bossHealthSystem;

    // Removed: 碰撞層級相關變數，因為不再需要複雜的碰撞忽略
    // private int platformLayerIndex;
    // private int defaultLayerIndex; 
    // private int originalBossLayerIndex;
    // private Collider2D bossOwnCollider;

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

        // Removed: Layer 索引獲取，因為不再需要複雜的碰撞忽略
        // platformLayerIndex = LayerMask.NameToLayer("Platform");
        // defaultLayerIndex = LayerMask.NameToLayer("Default");
        // originalBossLayerIndex = gameObject.layer; 
        // bossOwnCollider = GetComponent<Collider2D>();
        
        // Removed: Layer 警告，因為不再檢查這些 Layer
        // if (platformLayerIndex == -1) { Debug.LogWarning(...); }
        // if (defaultLayerIndex == -1) { Debug.LogWarning(...); }
    }

    private void Update()
    {
        if (GameProgressionManager.currentGameState != GameProgressionManager.GameState.Playing ||
            isPlayerDead ||
            bossHealthSystem == null || bossHealthSystem.IsDead)
            return;

        CheckBossPhase(); // 仍然需要檢查血量階段

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

    void CheckBossPhase()
    {
        if (isInPhase2 || bossHealthSystem == null) return;

        float currentHealthPercent = (float)bossHealthSystem.CurrentHealth / bossHealthSystem.MaxHealth;

        if (currentHealthPercent <= phase2HealthThreshold)
        {
            Debug.Log($"BossHealth: 血量低於 {phase2HealthThreshold:P0}，進入第二階段！攻擊頻率增加。");
            isInPhase2 = true;
            
            // === 新增：觸發畫面閃紅 ===
            if (ScreenFlashManager.Instance != null)
            {
                ScreenFlashManager.Instance.FlashScreen();
                Debug.Log("BossController: 觸發畫面閃紅效果。");
            }
            else
            {
                Debug.LogWarning("BossController: 未找到 ScreenFlashManager 實例！無法觸發畫面閃紅。請確保 RedFlashPanel 在場景中並掛載 ScreenFlashManager。");
            }
        }
    }

    IEnumerator JumpRoutine()
    {
        while (!isPlayerDead && bossHealthSystem != null && !bossHealthSystem.IsDead)
        {
            float currentMinInterval = jumpMinInterval;
            float currentMaxInterval = jumpMaxInterval;
            if (isInPhase2)
            {
                currentMinInterval *= phase2JumpFrequencyMultiplier;
                currentMaxInterval *= phase2JumpFrequencyMultiplier;
            }

            float waitTime = UnityEngine.Random.Range(currentMinInterval, currentMaxInterval);
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

            // 保持只在落地時震動，跳躍時不震動
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (bossHealthSystem != null && bossHealthSystem.IsDead) return;

        if (isJumping && collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;

            // Removed: isPhaseTransitioning 檢查，因為不再有複雜的階段位移
            // if (isPhaseTransitioning)
            // {
            //     Debug.Log("BossController: 階段過渡跳躍落地。不觸發常規落地效果。");
            // }
            // else
            // {
                if (cameraShake != null)
                    StartCoroutine(cameraShake.Shake(0.3f, 0.4f));

                BounceNearbyPlayers();
                SpawnShockwave();

                Debug.Log("Boss 落地，產生地震波！");
            // }
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
            Vector3 offset = new Vector3(-0.5f, shockwaveYOffset, 0f);
            Instantiate(shockwavePrefab, shockwaveSpawnPoint.position + offset, Quaternion.identity);
        }
    }

    IEnumerator ThrowRoutine()
    {
        while (!isPlayerDead && bossHealthSystem != null && !bossHealthSystem.IsDead)
        {
            float healthPercent = (float)bossHealthSystem.CurrentHealth / bossHealthSystem.MaxHealth; 

            float min = throwMinInterval;
            float max = throwMaxInterval;

            if (isInPhase2) // 根據是否進入第二階段調整投擲間隔
            {
                min *= phase2ThrowFrequencyMultiplier;
                max *= phase2ThrowFrequencyMultiplier;
            }

            float wait = UnityEngine.Random.Range(min, max);
            yield return new WaitForSeconds(wait);

            ThrowAtPlayer();
        }
    }

    void ThrowAtPlayer()
    {
        if (projectiles.Length == 0 || player == null || bossHealthSystem == null) return;

        GameObject selected = projectiles[UnityEngine.Random.Range(0, projectiles.Length)];
        GameObject proj = Instantiate(selected, throwPoint.position, Quaternion.identity);
        Collider2D bossCollider = GetComponent<Collider2D>();
        Collider2D projCollider = proj.GetComponent<Collider2D>();
        if (bossCollider != null && projCollider != null)
        {
            Physics2D.IgnoreCollision(projCollider, bossCollider);
        }

        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            Vector2 start = throwPoint.position;
            Vector2 target = player.position;
            float speed = throwForce;

            Vector2 velocity;
            if (CalculateArcVelocity(start, target, speed, out velocity))
            {
                projRb.linearVelocity = velocity;
            }
            else
            {
                // 備用方案：如果算不出拋物線（太近太遠），就朝方向亂丟
                Vector2 fallbackDir = (target - start).normalized;
                projRb.linearVelocity = fallbackDir * speed;
            }
        }
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
        if (knockbackObjectPrefab == null || player == null) return; 

        Transform spawnPoint = (knockbackThrowPoint != null) ? knockbackThrowPoint : throwPoint;
        if (spawnPoint == null) return;

        GameObject obj = Instantiate(knockbackObjectPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody2D objRb = obj.GetComponent<Rigidbody2D>(); 
        if (objRb != null)
        {
            Vector2 knockbackDir = (player.position - transform.position).normalized; 
            
            if (Mathf.Abs(knockbackDir.x) < 0.2f) { 
                knockbackDir.x = (knockbackDir.x >= 0) ? 0.2f : -0.2f; 
            }
            knockbackDir.y = Mathf.Max(0.2f, knockbackDir.y); 

            objRb.linearVelocity = knockbackDir * knockbackLaunchForce; 
        }

        Debug.Log("Boss 發動近距離擊退攻擊！");
    }

    IEnumerator KnockbackCooldownRoutine()
    {
        canKnockback = false; 
        yield return new WaitForSeconds(knockbackCooldown); 
        canKnockback = true; 
    }

    bool CalculateArcVelocity(Vector2 start, Vector2 end, float speed, out Vector2 velocity)
    {
        velocity = Vector2.zero;

        Vector2 dir = end - start;
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float sqrSpeed = speed * speed;
        float sqrSpeedSquared = sqrSpeed * sqrSpeed;
        float gx = dir.x;
        float gy = dir.y;

        float underRoot = sqrSpeed * sqrSpeed - gravity * (gravity * gx * gx + 2 * gy * sqrSpeed);

        if (underRoot < 0)
        {
            // 無解，目標太遠或太近，速度太小
            return false;
        }

        float root = Mathf.Sqrt(underRoot);

        // 用較高角度（比較好看）
        float highAngle = Mathf.Atan((sqrSpeed + root) / (gravity * gx));
        velocity = new Vector2(Mathf.Cos(highAngle), Mathf.Sin(highAngle)) * speed;

        if (gx < 0) velocity.x = -velocity.x;

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bounceRadius); 

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, knockbackRange); 
    }
}