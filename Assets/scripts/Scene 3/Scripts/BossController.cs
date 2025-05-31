using UnityEngine;
using System.Collections; // 使用協程 (IEnumerator, StartCoroutine, WaitForSeconds) 需要引入此命名空間

public class BossController : MonoBehaviour
{
    [Header("跳躍設定")]
    public float jumpForce = 20f; // Boss 跳躍的力度
    public float jumpMinInterval = 2f; // Boss 跳躍的最小間隔時間
    public float jumpMaxInterval = 4f; // Boss 跳躍的最大間隔時間
    public CameraShake cameraShake; // 引用攝影機震動腳本 (如果有的話，在 Inspector 中拖曳)
    public float bounceForce = 5f; // Boss 落地時彈起玩家的力度
    public float bounceRadius = 5f; // Boss 落地時偵測玩家並彈起的範圍 (紅色圓圈)
    public LayerMask platformLayer; // 指定平台所在的 Layer (在 Inspector 中勾選)
    public float platformDetectRadius = 3f; // Boss 落地時偵測附近可破壞平台的範圍 (藍色圓圈)

    [Header("投擲設定")]
    public GameObject[] projectiles; // Boss 投擲物的預製體陣列 (拖曳多個投擲物 Prefab 到這裡)
    public Transform throwPoint; // 投擲物發射點的 Transform (在 Boss 下創建一個空物件並拖曳到這裡)
    public float throwForce = 10f; // 投擲物的發射力度
    public float throwMinInterval = 2f; // 投擲的最小間隔時間
    public float throwMaxInterval = 5f; // 投擲的最大間隔時間

    private Rigidbody2D rb; // Boss 自身的 Rigidbody2D 元件
    private Transform player; // 玩家 Transform 的引用
    private bool isPlayerDead = false; // 玩家是否死亡
    private bool isJumping = false; // <<<<<< 新增：Boss 是否正在跳躍中的狀態變數 >>>>>>

    // 引用 Boss 自身的 BossHealth 腳本，用於判斷 Boss 死亡狀態
    private BossHealth bossHealthSystem; 

    // Unity 內建方法，在物件啟用時被呼叫 (常用於事件訂閱)
    private void OnEnable()
    {
        GameEvents.OnPlayerDied += OnPlayerDied; // 監聽玩家死亡事件
        GameEvents.OnGameStart += OnGameStart; // 監聽遊戲開始事件
    }

    // Unity 內建方法，在物件禁用或銷毀時被呼叫 (常用於取消事件訂閱)
    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= OnPlayerDied; // 取消監聽玩家死亡事件
        GameEvents.OnGameStart -= OnGameStart; // 取消監聽遊戲開始事件
    }

    // Unity 內建方法，在腳本第一次啟用時被呼叫 (常用於初始化)
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 獲取 Boss 自身的 Rigidbody2D
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // 找到場景中 Player 物件
        bossHealthSystem = GetComponent<BossHealth>(); // 獲取 Boss 自身的 BossHealth 腳本
    }

    // Unity 內建方法，每幀被呼叫一次 (常用於遊戲邏輯判斷)
    private void Update()
    {
        // 如果遊戲不在 "Playing" 狀態，或者玩家死亡，或者 Boss 已經死亡，則停止 Boss 的所有 Update 邏輯
        if (IntroManager.currentGameState != IntroManager.GameState.Playing || 
            isPlayerDead || 
            (bossHealthSystem != null && bossHealthSystem.IsDead())) 
            return;

        // 如果需要，可以在這裡添加 Boss 的其他 Update 邏輯，例如巡邏、瞄準等
    }

    // 當 GameEvents.OnPlayerDied 事件被觸發時，此方法會被呼叫
    private void OnPlayerDied()
    {
        Debug.Log("Boss 偵測到玩家死亡，停止行為！");
        isPlayerDead = true; // 設定玩家死亡標誌
        StopAllCoroutines(); // 停止 Boss 自身所有正在運行的協程 (例如跳躍和投擲)
        CancelInvoke(); // 防止任何通過 Invoke() 延遲呼叫的方法繼續執行
    }

    // 當 GameEvents.OnGameStart 事件被觸發時，此方法會被呼叫
    private void OnGameStart()
    {
        Debug.Log("BossController 偵測到遊戲開始，啟動 Boss 行為！");
        // 確保 Boss 和 Player 都還活著，才啟動 Boss 的跳躍和投擲協程
        if (!isPlayerDead && (bossHealthSystem != null && !bossHealthSystem.IsDead())) 
        {
            StartCoroutine(JumpRoutine()); // 啟動跳躍協程
            StartCoroutine(ThrowRoutine()); // 啟動投擲協程
        }
    }

    // --- Boss 跳躍邏輯 ---
    IEnumerator JumpRoutine()
    {
        // 只要玩家和 Boss 都活著，就一直執行跳躍邏輯
        while (!isPlayerDead && (bossHealthSystem != null && !bossHealthSystem.IsDead())) 
        {
            // 隨機等待一段時間，然後跳躍
            float waitTime = UnityEngine.Random.Range(jumpMinInterval, jumpMaxInterval); 
            yield return new WaitForSeconds(waitTime);
            Jump(); // 執行跳躍
        }
    }

    void Jump()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); 
            isJumping = true; // 設定 Boss 正在跳躍中
        }
        //Debug.Log("Boss 跳起來了！");
    }

    // Unity 內建方法，當此物件的碰撞體進入/離開另一個非 Trigger 碰撞體時被呼叫
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果 Boss 已經死亡，則不處理任何碰撞
        if (bossHealthSystem != null && bossHealthSystem.IsDead()) return;

        // 判斷 Boss 是否正在跳躍中，並且碰到了 Tag 為 "Ground" 的物件 (地面或平台)
        if (isJumping && collision.gameObject.CompareTag("Ground")) 
        {
            isJumping = false; // Boss 落地，重置跳躍狀態

            // 處理平台震動和掉落邏輯
            HandlePlatformShatter(collision.contacts[0].point); 

            // 如果有攝影機震動腳本，觸發攝影機震動
            if (cameraShake != null)
                StartCoroutine(cameraShake.Shake(0.3f, 0.4f));

            // 彈起附近的玩家
            BounceNearbyPlayers(); 
            //Debug.Log("Boss 落地！偵測並震動平台，並彈起玩家。"); 
        }
    }

    // 處理平台震動和掉落的邏輯
    void HandlePlatformShatter(Vector2 collisionPoint)
    {
        //Debug.Log("HandlePlatformShatter 被呼叫！碰撞點：" + collisionPoint); 

        // 在 Boss 落地點附近，偵測屬於 platformLayer 的所有 Collider2D
        Collider2D[] hitPlatforms = Physics2D.OverlapCircleAll(collisionPoint, platformDetectRadius, platformLayer); 

        if (hitPlatforms.Length == 0)
        {
            //Debug.Log("沒有偵測到任何平台在碰撞點附近。"); 
        }

        foreach (Collider2D platformCol in hitPlatforms)
        {
            //Debug.Log("偵測到物體在平台偵測範圍內：" + platformCol.name + " Tag: " + platformCol.tag); 

            // 如果偵測到的平台具有 "DestructiblePlatform" Tag
            if (platformCol.CompareTag("DestructiblePlatform")) 
            {
                //Debug.Log("偵測到 DestructiblePlatform: " + platformCol.name + "，嘗試呼叫 StartShatter。"); 
                // 獲取該平台的 DestructiblePlatform 腳本
                DestructiblePlatform destructiblePlatform = platformCol.GetComponent<DestructiblePlatform>();
                if (destructiblePlatform != null)
                {
                    destructiblePlatform.StartShatter(); // 呼叫平台的震動和掉落方法
                }
                else
                {
                    //Debug.LogWarning(platformCol.name + " 有 DestructiblePlatform Tag，但沒有 DestructiblePlatform 腳本！"); 
                }
            }
            else
                {
                    //Debug.Log("偵測到非 DestructiblePlatform 的物體: " + platformCol.name + " Tag: " + platformCol.tag); 
                }
        }
    }

    // 彈起附近玩家的邏輯
    void BounceNearbyPlayers()
    {
        // 偵測 Boss 周圍 bounceRadius 範圍內的 Collider2D
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, bounceRadius);

        foreach (Collider2D col in hits)
        {
            // 如果偵測到 Tag 為 "Player" 的物件
            if (col.CompareTag("Player"))
            {
                // 獲取 Player 的 Rigidbody2D
                Rigidbody2D playerRb = col.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // 施加一個向上的速度來彈起玩家
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce); // linearVelocity 已被棄用，改為 velocity
                    //Debug.Log("玩家被 Boss 彈起！");
                }
            }
        }
    }

    // --- Boss 投擲邏輯 ---
    IEnumerator ThrowRoutine()
    {
        // 只要玩家和 Boss 都活著，就一直執行投擲邏輯
        while (!isPlayerDead && (bossHealthSystem != null && !bossHealthSystem.IsDead())) 
        {
            // 隨機等待一段時間，然後投擲
            float wait = UnityEngine.Random.Range(throwMinInterval, throwMaxInterval); 
            yield return new WaitForSeconds(wait);
            ThrowAtPlayer(); // 執行投擲
        }
    }

    void ThrowAtPlayer()
    {
        // 檢查是否有投擲物預製體和玩家存在
        if (projectiles.Length == 0 || player == null) return;

        // 隨機選擇一個投擲物預製體
        GameObject selected = projectiles[UnityEngine.Random.Range(0, projectiles.Length)]; 
        // 在投擲點 Instantiate (實例化) 投擲物
        GameObject proj = Instantiate(selected, throwPoint.position, Quaternion.identity);

        // 計算從投擲點指向玩家當前位置的方向向量
        Vector2 dir = (player.position - throwPoint.position).normalized;
        // 獲取投擲物的 Rigidbody2D
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 施加力度，讓投擲物飛向玩家
            rb.linearVelocity = dir * throwForce; // linearVelocity 已被棄用，改為 velocity
        }
    }

    // --- Editor Gizmo 用於在 Scene 視窗中可視化偵測範圍 ---
    private void OnDrawGizmosSelected()
    {
        // 繪製紅色圓圈：表示彈起玩家的範圍
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bounceRadius);

        // 繪製藍色圓圈：表示偵測平台並震動的範圍
        if (GetComponent<Collider2D>() != null) 
        {
            Gizmos.color = Color.blue;
            // 計算 Boss 腳底的位置作為圓圈中心
            Vector2 gizmoCenter = (Vector2)transform.position - new Vector2(0, GetComponent<Collider2D>().bounds.extents.y);
            Gizmos.DrawWireSphere(gizmoCenter, platformDetectRadius); 
        }
    }
}