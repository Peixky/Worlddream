using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public float shootInterval = 2f;
    public float bulletSpeed = 5f;
    public float shootingRange = 10f;

    [Header("Player Collision Damage")]
    public int contactDamage = 1;

    private Transform player;
    private float timer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        // ✅ 朝向玩家
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * -Mathf.Sign(player.position.x - transform.position.x);
        transform.localScale = scale;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ✅ 射擊判斷
        if (distanceToPlayer <= shootingRange)
        {
            timer += Time.deltaTime;
            if (timer >= shootInterval)
            {
                Shoot();
                timer = 0f;
            }
        }
    }

    void Shoot()
    {
        if (player == null || bulletPrefab == null) return;

        // 正確取得射擊方向（只管位置差）
        Vector2 direction = (player.position - transform.position).normalized;

        // 子彈生成位置偏移（向玩家方向偏一點）
        Vector3 spawnPos = transform.position + (Vector3)(direction * 0.5f); 

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }
    }
}
