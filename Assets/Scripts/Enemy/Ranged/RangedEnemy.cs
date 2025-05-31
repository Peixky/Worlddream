using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float shootInterval = 2f;
    public float bulletSpeed = 5f;
    public float shootingRange = 10f; // 玩家進入這個距離才會開火

    private Transform player;
    private float timer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // ✅ 朝向玩家（轉向）
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * -Mathf.Sign(player.position.x - transform.position.x);
        transform.localScale = scale;

        // ✅ 計算玩家與敵人的距離
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ✅ 玩家進入射程才會進行射擊計時與射擊
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
        if (player == null) return;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed; // ✅ 建議使用 velocity 而非 linearVelocity（更常見用法）
    }
}
