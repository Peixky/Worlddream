using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class RangedChaserEnemy : MonoBehaviour
{
    [Header("攻擊設定")]
    public float shootInterval = 2f;
    public float bulletSpeed = 5f;
    public float shootingRange = 8f;
    public GameObject bulletPrefab;

    [Header("移動設定")]
    public float moveSpeed = 2f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private float shootTimer;
    private bool isShooting = false;

    [Header("動畫設定")]
    public float shootAnimDuration = 1.2f;      // 投擲動畫總長度
    public float bulletFireDelay = 0.5f;        // 幾秒後發射子彈（動畫中段）

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > shootingRange)
        {
            if (!isShooting)
            {
                Vector2 dir = (player.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);
                animator.SetBool("IsWalking", true);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsWalking", false);

            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval && !isShooting)
            {
                StartCoroutine(ShootSequence());
                shootTimer = 0f;
            }
        }

        FlipToFacePlayer();
    }

    IEnumerator ShootSequence()
    {
        isShooting = true;

        // 播放投擲動畫
        if (animator != null)
        {
            //animator.SetTrigger("Shoot");
        }

        // 等到動畫中段再發射子彈
        yield return new WaitForSeconds(bulletFireDelay);
        FireBullet();

        // 等待動畫播完
        yield return new WaitForSeconds(shootAnimDuration - bulletFireDelay);

        isShooting = false;
    }

    void FireBullet()
    {
        if (bulletPrefab == null || player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        Vector3 spawnPos = transform.position + new Vector3(direction.x * 0.5f, 0, 0);

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = direction * bulletSpeed;
        }

        Debug.Log("敵人射擊子彈！");
    }

    void FlipToFacePlayer()
    {
        Vector3 scale = transform.localScale;
        scale.x = -Mathf.Abs(scale.x) * Mathf.Sign(player.position.x - transform.position.x);
        transform.localScale = scale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Health health = collision.collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(1);
            }
        }
    }
}
