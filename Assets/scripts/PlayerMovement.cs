using UnityEngine;

public class Playermovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;  // 玩家移動速度
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D body;  // 角色的 2D 剛體
    private BoxCollider2D boxCollider;  // 角色的 2D 碰撞器
    private Animator anim;
    private bool grounded;

    private void Awake(){
        // 初始化
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update(){
        // 獲取水平輸入值（例如，A 或 D 鍵，或方向鍵）
        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

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

        if (Input.GetKeyDown(KeyCode.Space) && grounded) {
            jump();
        }
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", grounded);
    }
    private void jump(){
        body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
        //anim.SetTrigger("jump");
        anim.SetTrigger("jump");
        grounded = false;
    }
    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Ground"){
            grounded = true;
        }
    }
    /*private bool isGrounded(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return false;
    }*/
}
