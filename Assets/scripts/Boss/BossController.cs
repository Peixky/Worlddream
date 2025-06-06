using UnityEngine;
using System.Collections; // 確保有這個命名空間，用於協程
using System; // 確保有這個命名空間，用於 Action 事件 (給 Lambda 用)

[RequireComponent(typeof(Rigidbody2D), typeof(Health))]
public class BossController : MonoBehaviour
{
    [Header("跳躍設定")]
    public float jumpForce = 20f;
    public float jumpMinInterval = 2f;
    public float jumpMaxInterval = 4f;
    public CameraShake cameraShake; // 假設 CameraShake 腳本存在
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
    public Transform knockbackThrowPoint; 
    public float knockbackRange = 3f;
    public float knockbackCooldown = 5f;
    public float knockbackLaunchForce = 10f; // 獨立的擊退力道

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
        // 確保玩家物件有 "Player" Tag
        player = GameObject.FindGameObjectWithTag("Player")?.transform; 
        bossHealthSystem = GetComponent<Health>();
    }

    private void Update()
    {
        // 檢查全局遊戲狀態，只有在 Playing 時 Boss 才行動
        // 使用 GameProgressionManager.currentGameState，因為它管理全局狀態
        if (GameProgressionManager.currentGameState != GameProgressionManager.GameState.Playing || 
            isPlayerDead || // 玩家死亡時停止
            bossHealthSystem == null || bossHealthSystem.IsDead) // Boss 死亡時停止
            return;

        // 檢查近距離擊退攻擊
        CheckForKnockbackAttack();
    }

    private void OnGameStart()
    {
        Debug.Log("✅ BossController 啟動成功！");
        // Boss 可以在遊戲開始時啟動行為協程
        StartCoroutine(JumpRoutine());
        StartCoroutine(ThrowRoutine());
    }

    private void OnPlayerDied()
    {
        Debug.Log("Boss 偵測到玩家死亡，停止行為！");
        isPlayerDead = true;
        // 停止所有 Boss 的攻擊協程和 Invoke
        StopAllCoroutines();
        CancelInvoke();
    }

    IEnumerator JumpRoutine()
    {
        // 只有在玩家活著且 Boss 活著時才執行跳躍行為
        while (!isPlayerDead && bossHealthSystem != null && !bossHealthSystem.IsDead)
        {
            float waitTime = UnityEngine.Random.Range(jumpMinInterval, jumpMaxInterval);
            yield return new WaitForSeconds(waitTime);
            Jump();
        }
    }

    void Jump()
    {
        if (rb != null)
        {
            // 給予跳躍力道
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true; // 標記為正在跳躍
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Boss 死亡後不再觸發落地行為
        if (bossHealthSystem != null && bossHealthSystem.IsDead) return;

        // 當 Boss 跳躍後落地且撞到地面時
        if (isJumping && collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false; // 標記為已落地

            // 如果有攝影機震動組件，則觸發震動
            if (cameraShake != null)
                StartCoroutine(cameraShake.Shake(0.3f, 0.4f));

            // 觸發擊退附近玩家和產生地震波
            BounceNearbyPlayers();
            SpawnShockwave();

            Debug.Log("Boss 落地，產生地震波！");
        }
    }

    void BounceNearbyPlayers()
    {
        // 查找範圍內所有與 Boss 重疊的 Collider2D
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, bounceRadius);

        foreach (Collider2D col in hits)
        {
            // 如果是玩家，則給予向上的擊退力道
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
            Vector3 offset = new Vector3(-0.5f, 0f, 0f); // 調整地震波生成位置
            Instantiate(shockwavePrefab, shockwaveSpawnPoint.position + offset, Quaternion.identity);
        }
    }

    IEnumerator ThrowRoutine()
    {
        // 只有在玩家活著且 Boss 活著時才執行投擲行為
        while (!isPlayerDead && bossHealthSystem != null && !bossHealthSystem.IsDead)
        {
            // 計算 Boss 血量百分比 (修正浮點數除法)
            float healthPercent = (float)bossHealthSystem.CurrentHealth / bossHealthSystem.MaxHealth; 

            // 根據血量調整投擲間隔
            float min = throwMinInterval;
            float max = throwMaxInterval;

            if (healthPercent <= 0.3f) // 血量低於 30%
            {
                min *= 0.4f;
                max *= 0.6f;
            }
            else if (healthPercent <= 0.5f) // 血量低於 50%
            {
                min *= 0.6f;
                max *= 0.8f;
            }

            float wait = UnityEngine.Random.Range(min, max);
            yield return new WaitForSeconds(wait);

            ThrowAtPlayer();
        }
    }

    void ThrowAtPlayer()
    {
        if (projectiles.Length == 0 || player == null || bossHealthSystem == null) return;

        // 再次計算血量百分比 (確保最新值，並修正浮點數除法)
        float healthPercent = (float)bossHealthSystem.CurrentHealth / bossHealthSystem.MaxHealth; 

        // 根據血量調整投擲物數量
        int numberOfProjectiles = 1;
        if (healthPercent <= 0.3f)
            numberOfProjectiles = 3;
        else if (healthPercent <= 0.5f)
            numberOfProjectiles = 2;

        // 計算投擲方向
        Vector2 baseDir = (player.position - throwPoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        float totalSpreadAngle = 30f; // 投擲物散開的總角度
        float angleStep = (numberOfProjectiles > 1) ? totalSpreadAngle / (numberOfProjectiles - 1) : 0f;
        float startAngle = baseAngle - totalSpreadAngle / 2f;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angle = startAngle + i * angleStep;
            float radian = angle * Mathf.Deg2Rad;
            Vector2 throwDir = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;

            // 隨機選擇投擲物並實例化
            GameObject selected = projectiles[UnityEngine.Random.Range(0, projectiles.Length)];
            GameObject proj = Instantiate(selected, throwPoint.position, Quaternion.identity);

            Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>(); // 獲取投擲物的 Rigidbody2D
            if (projRb != null)
            {
                projRb.linearVelocity = throwDir * throwForce; // 施加投擲力道
            }
        }

        Debug.Log($"Boss 血量：{healthPercent:P0}，投擲 {numberOfProjectiles} 顆，方向扇形展開！");
    }

    // ===== 近距離擊退判定 =====
    void CheckForKnockbackAttack()
    {
        if (!canKnockback || player == null) return;

        // 檢查 Boss 與玩家的距離
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= knockbackRange)
        {
            KnockbackPlayer(); // 如果在擊退範圍內，發動擊退攻擊
            StartCoroutine(KnockbackCooldownRoutine()); // 啟動擊退冷卻
        }
    }

    void KnockbackPlayer()
    {
        // 確保預製件和玩家存在
        if (knockbackObjectPrefab == null || player == null) return; 

        // 選擇投射點
        Transform spawnPoint = (knockbackThrowPoint != null) ? knockbackThrowPoint : throwPoint;
        if (spawnPoint == null) return;

        // 實例化擊退物體
        GameObject obj = Instantiate(knockbackObjectPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody2D objRb = obj.GetComponent<Rigidbody2D>(); 
        if (objRb != null)
        {
            // 計算從 Boss 推開玩家的方向 (Boss在玩家右邊，所以推向左邊)
            // 從 Boss 指向玩家的方向向量，其 X 分量為負，符合向左推的邏輯
            Vector2 knockbackDir = (player.position - transform.position).normalized; 
            
            // 確保水平方向有足夠的力道，避免太垂直的擊退
            // 如果 Boss 在玩家右邊，knockbackDir.x 會是負數。這裡會確保向左有至少 0.2f 的力道。
            if (Mathf.Abs(knockbackDir.x) < 0.2f) { 
                knockbackDir.x = (knockbackDir.x >= 0) ? 0.2f : -0.2f; // 強制向左或向右最小力道
            }
            // 確保至少有向上推的力道，避免直接平推
            knockbackDir.y = Mathf.Max(0.2f, knockbackDir.y); 

            // 施加擊退力道
            objRb.linearVelocity = knockbackDir * knockbackLaunchForce; 
        }

        Debug.Log("Boss 發動近距離擊退攻擊！");
    }

    IEnumerator KnockbackCooldownRoutine()
    {
        canKnockback = false; // 進入冷卻
        yield return new WaitForSeconds(knockbackCooldown); // 等待冷卻時間
        canKnockback = true; // 結束冷卻
    }

    // 在編輯器中選擇 Boss 時，繪製輔助線以顯示範圍
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bounceRadius); // 顯示落地反彈範圍

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, knockbackRange); // 顯示近距離擊退範圍
    }
}