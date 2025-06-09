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

    // === 修改後的拋物線計算函式 ===
    // 參考資料：https://gamedev.stackexchange.com/questions/112521/how-can-i-predict-projectiles-trajectory-with-unity
    // 或：https://unity3d.com/learn/tutorials/topics/physics/projectile-launch
    bool CalculateThrowVelocity(Vector2 start, Vector2 target, float speed, float gravity, out Vector2 velocity)
    {
        velocity = Vector2.zero;

        // 計算水平和垂直距離
        float dx = target.x - start.x;
        float dy = target.y - start.y;

        // 計算到達目標所需的時間（假設水平速度是恆定的）
        // 如果dx為0，代表正上方或正下方，這時拋物線公式會出錯，需要額外處理
        if (Mathf.Approximately(dx, 0f))
        {
            // 如果目標在正上方或正下方，則直接向上或向下投擲
            velocity = new Vector2(0f, Mathf.Sign(dy) * speed); // 直接向上或向下，速度為speed
            // 注意：這種情況下可能不是拋物線，如果需要拋物線，則必須有水平位移
            // 這裡簡單處理，如果是精確垂直，則直接向上/下，否則給予極小水平偏移
            if (Mathf.Abs(dy) > 0)
            {
                // 如果目標在正上方，考慮重力影響，給予向上速度
                // 對於垂直目標，無法計算拋物線，需要額外處理，這裡簡單處理為直接向上
                // 為了讓它能動，給一個足夠大的向上速度，但可能不是完美拋物線
                // 更穩健的做法是，如果目標在正上方，讓Boss投擲一個會垂直向上然後下落的物體
                // 但為了通用性，我們確保dx不為0來走拋物線計算
                dx = 0.01f * Mathf.Sign(target.x - start.x); // 給予一個極小的水平位移
            } else {
                return false; // 如果dx和dy都為0，目標在原地，無法投擲
            }
        }
        
        // 確保水平速度不為0
        float timeToReachTarget = Mathf.Abs(dx / speed); // 先用直線速度估算一個時間

        // 嘗試計算一個能夠到達目標的水平速度分量
        // 如果垂直距離是正的（目標更高），我們可能需要考慮垂直方向的初速度
        // 如果垂直距離是負的（目標更低），則重力會幫助下落

        // 使用公式：
        // vy = (dy + 0.5 * gravity * t^2) / t
        // vx = dx / t
        
        // 這裡的 't' 是一個關鍵。由於我們不知道確切的 't'，我們可以迭代或使用更複雜的代數。
        // 然而，最簡單的方式是確保有一個初始速度分量，然後讓物理引擎完成其餘的工作。
        // 為了確保 projectile 能到達目標，我們可以計算一個最小的水平速度
        // 讓 projectile 在空中停留足夠長的時間，以便重力將其拉到目標高度
        
        // 重新使用一個更通用的方法，它不直接依賴於一個固定的 `speed` 來計算 `t`，
        // 而是找到一個初速度向量。
        
        // 此處參考 Unity 官方的拋射物體教程中的代數解法
        // Source: https://unity3d.com/learn/tutorials/topics/physics/projectile-launch
        
        float v_x = dx / timeToReachTarget; // 水平速度分量
        float v_y = (dy + 0.5f * Mathf.Abs(gravity) * timeToReachTarget * timeToReachTarget) / timeToReachTarget; // 垂直速度分量
        
        velocity = new Vector2(v_x, v_y);

        // 如果計算出的速度大小與我們期望的 `speed` 差距過大，
        // 說明這個 `timeToReachTarget` 估計不合理。
        // 更好的做法是，固定速度 `speed`，然後計算角度。
        // 這裡提供一個更穩健，可以確保總是能到達目標的投擲，即使不是完美拋物線，
        // 但會更可靠。

        // 重新設計為直接計算到達目標所需的初始速度 (忽略 `speed` 作為投擲初速度的限制)
        // 而是讓 `speed` 成為一個“力量係數”
        float angle = 0f; // 角度

        // 如果要確保投擲速度為 `speed`，則需要計算角度
        // 這是一個更穩定的拋物線計算，確保總能擊中目標，即使角度可能不是最優雅的
        // 這會找到一個可以命中目標的發射角度，且發射速度為speed
        float a = target.x - start.x;
        float b = target.y - start.y;
        float g = Mathf.Abs(gravity);
        float v = speed; // 發射速度

        float v2 = v * v;
        float v4 = v2 * v2;
        float discriminant = v4 - g * (g * a * a + 2 * b * v2);

        if (discriminant < 0)
        {
            // 目標無法到達，速度太小。
            // 這種情況下，我們仍然會給一個方向，讓它儘可能接近。
            // 為了確保一定能射中，我們可以稍微降低對速度的堅持
            // 或者，如果目標距離太遠，直接直線投擲
            Debug.LogWarning("Target unreachable with given speed, using alternative calculation.");
            velocity = (target - start).normalized * speed; // 使用直線投擲作為備用
            return true; 
        }

        float time1 = (v2 + Mathf.Sqrt(discriminant)) / (g * Mathf.Abs(a)); // 兩種時間
        float time2 = (v2 - Mathf.Sqrt(discriminant)) / (g * Mathf.Abs(a));

        float bestTime = (time1 < time2 && time1 > 0) ? time1 : time2; // 選取較小且為正的時間

        if (bestTime <= 0) // 如果兩個時間都不符合，可能需要更簡單的估計
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