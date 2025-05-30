using UnityEngine;

public class Playermovement : MonoBehaviour
{
    [Header("移動參數")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float wallJumpX = 5f;
    [SerializeField] private float wallJumpY = 10f;
    [SerializeField] private float wallSlideSpeed = -2f; // 貼牆下滑速度

    [Header("偵測區域與層級")]
    [SerializeField] private LayerMask groundLayer; // 專用於地面檢測的 Layer
    [SerializeField] private LayerMask wallLayer;   // 專用於牆壁檢測的 Layer
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck; // 只有一個 wallCheck
    [SerializeField] private float checkRadius = 0.2f;

    [Header("壁跳鎖定時間")]
    [SerializeField] private float wallJumpDuration = 0.2f; // 壁跳後無法水平控制的時間

    private Rigidbody2D body;
    private Animator anim;
    private float horizontalInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallJumping;
    private float wallJumpTimer;

    public bool canMove = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!canMove)
        {
            body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
            anim.SetBool("run", false);
            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 當 wall jump 時不允許水平控制，直到壁跳計時器結束
        if (!isWallJumping || wallJumpTimer <= 0)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);
        }

        // 面向修正
        if (!isWallJumping) // 通常壁跳時不需要立即翻面，等壁跳結束再根據輸入翻面
        {
            if (horizontalInput > 0.01f)
                transform.localScale = new Vector3(1, 1, 1);
            else if (horizontalInput < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        // 跳躍 & 壁跳邏輯
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
                anim.SetTrigger("jump");
            }
            else if (isTouchingWall && !isGrounded) // 確保不在地面，且接觸到牆壁才能壁跳
            {
                // 壁跳方向與當前面向相反
                WallJump(-transform.localScale.x);
            }
        }

        // 動畫參數
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        // 使用 wallLayer 檢測牆壁
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer);

        // *** 關鍵修改：處理貼牆下滑 (移除 !isWallJumping 限制) ***
        // 只要接觸牆壁且不在地面，就允許貼牆下滑
        if (isTouchingWall && !isGrounded)
        {
            // 將垂直速度限制在 wallSlideSpeed (一個負值)
            body.linearVelocity = new Vector2(body.linearVelocity.x, Mathf.Max(body.linearVelocity.y, wallSlideSpeed));

            // 如果角色在壁跳中且現在碰到了牆壁，則立即結束壁跳狀態 (恢復水平控制)
            // 這樣可以讓角色一碰到新牆就立刻可以再次輸入，準備下一次壁跳
            if (isWallJumping)
            {
                isWallJumping = false;
                wallJumpTimer = 0f; // 清空計時器
            }
        }

        // 壁跳鎖定計時器，當時間結束時，確保 isWallJumping 為 false
        if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                isWallJumping = false;
            }
        }
    }

    private void WallJump(float direction)
    {
        isWallJumping = true; // 設置壁跳狀態為 true
        wallJumpTimer = wallJumpDuration; // 重置壁跳鎖定計時器

        body.linearVelocity = new Vector2(direction * wallJumpX, wallJumpY);
        anim.SetTrigger("jump");

        // 壁跳時，角色立即翻轉面向跳躍方向
        transform.localScale = new Vector3(direction, 1, 1);

        // 不再使用 Invoke，改為在 FixedUpdate 中根據 wallJumpTimer 判斷結束
        // Invoke(nameof(StopWallJump), wallJumpDuration); // 移除這行
    }
}