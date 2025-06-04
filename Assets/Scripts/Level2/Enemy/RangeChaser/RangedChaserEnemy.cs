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
    private float shootTimer;
    private bool isShooting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
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
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

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

        // 立即發射子彈
        FireBullet();

        // 簡單延遲，模擬射擊冷卻
        yield return new WaitForSeconds(0.1f);

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
}
