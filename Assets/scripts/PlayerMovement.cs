using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;  // 玩家移動速度
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;  // 角色的 2D 剛體
    private BoxCollider2D boxCollider;  // 角色的 2D 碰撞器
    private Animator anim;
    private float wallJumpCooldown;
    private float horizontalInput;
    public bool canMove = true; // 預設可以移動


    private void Awake(){
        // 初始化
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update(){
        if (!canMove)
            return;
        // 獲取水平輸入值（例如，A 或 D 鍵，或方向鍵）
        horizontalInput = Input.GetAxis("Horizontal");

        // 面向修正（右為正，左為負）
        if (horizontalInput > 0.01f) {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x); // 面向右
            transform.localScale = scale;
        }else if (horizontalInput < -0.01f) {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x); // 面向左
            transform.localScale = scale;
        }

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
        
        if(wallJumpCooldown > 0.2f){
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

    private void Start()
    {
        canMove = true; // 確保角色一開始可以動
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

    private bool onWall(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack(){
        return horizontalInput == 0 && isGrounded() && !onWall();
    }

    private void EnableMovement()
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null)
            pm.canMove = true;
    }

    public void Bounce(Vector2 force)
    {
        body.linearVelocity = Vector2.zero; // 重設原本速度
        body.AddForce(force, ForceMode2D.Impulse);
    }


}
