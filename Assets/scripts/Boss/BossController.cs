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
    public float phase2JumpFrequencyMultiplier = 0.5f; // 第二階段跳躍頻率乘數 (例如 0.5 表示頻率加倍)
    public float phase2ThrowFrequencyMultiplier = 0.5f; // 第二階段投擲頻率乘數
    
    private bool isInPhase2 = false;
    // ====================================

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
            Vector3 offset = new Vector3(-0.5f, shockwaveYOffset, 0f);
            Instantiate(shockwavePrefab, shockwaveSpawnPoint.position + offset, Quaternion.identity);
        }
    }

    IEnumerator ThrowRoutine()
    {
        while (!isPlayerDead && bossHealthSystem != null && !bossHealthSystem.IsDead)
        {
            // float healthPercent = (float)bossHealthSystem.CurrentHealth / bossHealthSystem.MaxHealth; // 這個變數目前沒有被使用到

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

            // 使用新的 CalculateThrowVelocity 函式，它會更穩定地計算出軌跡
            Vector2 velocity;
            if (CalculateThrowVelocity(start, target, speed, Physics2D.gravity.y, out velocity))
            {
                projRb.linearVelocity = velocity;
            }
            else
            {
                // 備用方案：如果真的無法計算（理論上不會發生，但以防萬一），則朝直線方向丟
                Vector2 fallbackDir = (target - start).normalized;
                projRb.linearVelocity = fallbackDir * speed;
                Debug.LogWarning("BossController: CalculateThrowVelocity 無法計算拋物線，使用直線備用方案。", proj);
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
            
            // 確保擊退方向有足夠的 X 和 Y 分量，避免完全水平或向下擊退
            if (Mathf.Abs(knockbackDir.x) < 0.2f) { // 避免 X 分量太小
                knockbackDir.x = (knockbackDir.x >= 0) ? 0.2f : -0.2f; 
            }
            knockbackDir.y = Mathf.Max(0.2f, knockbackDir.y); // 確保 Y 分量至少為 0.2f，避免向下擊退

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

    bool CalculateThrowVelocity(Vector2 start, Vector2 target, float speed, float gravity, out Vector2 velocity)
    {
        velocity = Vector2.zero;

        // 計算水平和垂直距離
        float dx = target.x - start.x;
        float dy = target.y - start.y;
        if (Mathf.Approximately(dx, 0f))
        {
            // 如果目標在正上方或正下方，則直接向上或向下投擲
            velocity = new Vector2(0f, Mathf.Sign(dy) * speed); // 直接向上或向下，速度為speed
            
            if (Mathf.Abs(dy) > 0)
            {
                dx = 0.01f * Mathf.Sign(target.x - start.x); // 給予一個極小的水平位移
            } else {
                return false; // 如果dx和dy都為0，目標在原地，無法投擲
            }
        }
        
        // 確保水平速度不為0
        float timeToReachTarget = Mathf.Abs(dx / speed); // 先用直線速度估算一個時間
        
        float v_x = dx / timeToReachTarget; // 水平速度分量
        float v_y = (dy + 0.5f * Mathf.Abs(gravity) * timeToReachTarget * timeToReachTarget) / timeToReachTarget; // 垂直速度分量
        
        velocity = new Vector2(v_x, v_y);
        float angle = 0f; // 角度
        float a = target.x - start.x;
        float b = target.y - start.y;
        float g = Mathf.Abs(gravity);
        float v = speed; // 發射速度

        float v2 = v * v;
        float v4 = v2 * v2;
        float discriminant = v4 - g * (g * a * a + 2 * b * v2);

        if (discriminant < 0)
        {
            Debug.LogWarning("Target unreachable with given speed, using alternative calculation.");
            velocity = (target - start).normalized * speed; // 使用直線投擲作為備用
            return true; 
        }

        float time1 = (v2 + Mathf.Sqrt(discriminant)) / (g * Mathf.Abs(a)); // 兩種時間
        float time2 = (v2 - Mathf.Sqrt(discriminant)) / (g * Mathf.Abs(a));

        float bestTime = (time1 < time2 && time1 > 0) ? time1 : time2; // 選取較小且為正的時間

        if (bestTime <= 0)
        {
            bestTime = Mathf.Abs(a) / speed; // 簡單的水平時間估計
            if (bestTime <= 0) bestTime = 0.1f; // 避免除以零
        }


        // 計算最終速度向量
        float vx = a / bestTime;
        float vy = (b + 0.5f * g * bestTime * bestTime) / bestTime;

        velocity = new Vector2(vx, vy);

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