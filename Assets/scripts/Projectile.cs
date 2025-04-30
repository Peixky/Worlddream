using UnityEngine;

public class Projectile : MonoBehaviour{
    [SerializeField] private float speed;
    private float direction;
    private bool hit;

    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake(){
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update(){
        if(hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision){
        // 碰到 Player 自己就跳過
        if (collision.CompareTag("Player")) return;

        // 碰到牆壁或敵人就爆炸
        if (collision.CompareTag("Wall") || collision.CompareTag("Enemy"))
        {
            hit = true;
            boxCollider.enabled = false;
            anim.SetTrigger("explode");
        }
    }


    public void SetDirection(float _direction){
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if(Mathf.Sign(localScaleX) != _direction){
            localScaleX = -localScaleX;
        }
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivated(){
        gameObject.SetActive(false);
    }
}
