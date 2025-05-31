using UnityEngine;

public class PlayerMovement : MonoBehaviour
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
            if(onWall() && !isGrounded()){
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }else{
                body.gravityScale = 3;
            }
            if(Input.GetKeyDown(KeyCode.Space) && isGrounded()){
                jump();
            }
        }else{
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void jump(){
        if(isGrounded()){
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("jump");
        }else if(onWall()){
            if(horizontalInput == 0){
                body.linearVelocity = new Vector2((-Mathf.Sign(transform.localScale.x)) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }else{
                body.linearVelocity = new Vector2((-Mathf.Sign(transform.localScale.x)) * 3, 6);
            }
            wallJumpCooldown = 0;
        }
        
    }
    private bool isGrounded(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
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