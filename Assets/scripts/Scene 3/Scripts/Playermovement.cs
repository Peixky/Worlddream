using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer; 

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool facingRight = true;

    private float moveInput;

    // <<<<<< 新增：回彈相關變數 >>>>>>
    private bool isRecoiling = false; 
    private Vector3 lastIdlePosition; // 儲存上一個靜止位置
    private Coroutine recoilCoroutine; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        groundLayer = LayerMask.GetMask("Ground", "Platform"); 
        
        //Debug.Log("PlayerMovement: groundLayer 設定為: " + groundLayer.value); 
        //Debug.Log("PlayerMovement: 'Ground' Layer 的數值是: " + LayerMask.NameToLayer("Ground"));
        //Debug.Log("PlayerMovement: 'Platform' Layer 的數值是: " + LayerMask.NameToLayer("Platform"));

        // 初始化 lastIdlePosition
        lastIdlePosition = transform.position;
    }

    void Update()
    {
        // <<<<<< 如果正在回彈，則停止玩家的普通移動和跳躍輸入 >>>>>>
        if (isRecoiling)
        {
            return; 
        }

        moveInput = Input.GetAxis("Horizontal");

        bool currentIsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded != currentIsGrounded) 
        {
            isGrounded = currentIsGrounded;
            //Debug.Log("PlayerMovement: isGrounded 狀態改變為: " + isGrounded + "。當前位置: " + groundCheck.position);
        }
        
        anim.SetBool("run", Mathf.Abs(moveInput) > 0.01f);
        anim.SetBool("grounded", isGrounded);

        // <<<<<< 偵測玩家是否靜止並更新 lastIdlePosition >>>>>>
        // 判斷靜止條件：在地面上且沒有水平移動
        if (isGrounded && Mathf.Abs(moveInput) < 0.01f && Mathf.Abs(rb.linearVelocity.x) < 0.1f) // 加上速度判斷更精確
        {
            // 只有當位置有明顯變化時才更新，避免每幀更新
            if (Vector3.Distance(lastIdlePosition, transform.position) > 0.1f)
            {
                lastIdlePosition = transform.position;
                //Debug.Log("PlayerMovement: 記錄新的靜止位置: " + lastIdlePosition);
            }
        }


        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded) 
        {
            //Debug.Log("PlayerMovement: 'W' 或 '上方向鍵' 被按下，且在地面上。準備跳躍。");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("jump"))
            {
                anim.SetTrigger("jump");
            }
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            //Debug.LogWarning("PlayerMovement: 'W' 或 '上方向鍵' 按下，但 isGrounded 為 FALSE。無法跳躍。");
        }

        if ((moveInput > 0 && !facingRight) || (moveInput < 0 && facingRight))
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        if (isRecoiling)
        {
            return; 
        }
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // <<<<<< 新增：可視化最後靜止位置 >>>>>>
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lastIdlePosition, 0.3f); // 在靜止位置畫一個藍色小圓圈
    }

    // <<<<<< 新增：回彈相關方法 >>>>>>

    // 啟動回彈的公共方法，PlayerAttack 腳本會呼叫它
    public void StartRecoilToLastIdle(float duration) // 只需要回彈時間
    {
        if (isRecoiling) return; 

        // 如果協程正在運行，先停止它
        if (recoilCoroutine != null)
        {
            StopCoroutine(recoilCoroutine);
        }
        recoilCoroutine = StartCoroutine(RecoilRoutine(duration));
    }

    // 協程：處理回彈的平滑移動
    IEnumerator RecoilRoutine(float duration)
    {
        isRecoiling = true; // 設定為回彈狀態
        rb.linearVelocity = Vector2.zero; // 清除 Player 當前速度，讓回彈移動更受控
        rb.angularVelocity = 0f; // 清除角速度
        rb.gravityScale = 0; // 暫時移除重力，確保平滑移動到目標位置
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 凍結旋轉，但允許位置移動

        Vector3 currentRecoilStartPos = transform.position; // 記錄回彈開始時的當前位置

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(currentRecoilStartPos, lastIdlePosition, elapsedTime / duration);
            
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        // 確保最終位置精確到達
        transform.position = lastIdlePosition;
        
        // 恢復 Rigidbody2D 設置
        rb.gravityScale = 1; // 恢復重力 (或你遊戲預設的重力)
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 恢復預設約束 (凍結Z旋轉)
        
        isRecoiling = false; // 回彈結束
        recoilCoroutine = null; 
        //Debug.Log("Player 回彈結束，回到靜止位置: " + lastIdlePosition);
    }

    // 提供給 PlayerAttack 腳本查詢 Player 是否正在回彈
    public bool IsRecoilActive()
    {
        return isRecoiling;
    }
}