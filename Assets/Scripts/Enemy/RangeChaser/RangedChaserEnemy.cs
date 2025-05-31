using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class RangedChaserEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float shootInterval = 2f;
    public float bulletSpeed = 5f;
    public GameObject bulletPrefab;

    private Transform player;
    private Rigidbody2D rb;
    private float shootTimer;

    public float shootingRange = 8f;

    private Animator animator; // 加入動畫控制器


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

    }

    void FixedUpdate()
    {
        if (player == null) return;

        float speed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", speed);


        // 追蹤玩家
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        Flip();
    }

    void Update()
    {
        if (player == null) return;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= shootingRange)
            {
                Shoot();
            }
            shootTimer = 0f;
        }     
    }

    void Shoot()
    {
        if (player == null || bulletPrefab == null) return;

        // 播放丟子彈動畫
        animator.SetTrigger("Attack");

        // 建立子彈
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector2 dir = new Vector2(player.position.x - transform.position.x, 0).normalized;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
    }


    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = (player.position.x > transform.position.x) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Health playerHealth = collision.collider.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // 近身碰撞傷害
            }
        }
    }
}
