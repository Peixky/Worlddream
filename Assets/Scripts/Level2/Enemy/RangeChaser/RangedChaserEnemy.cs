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
    public float boostSpeed = 4f;          // 加速速度
    public float boostRange = 5f;          // 觸發加速的距離
    public float chaseRange = 12f;
    public float chaseAcceleration = 10f;

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
        shootTimer += Time.deltaTime; // ✅ 始終累加計時器

        // 玩家太遠 → 停止
        if (distance > chaseRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 玩家在追擊範圍內但未進入射擊距離 → 追擊
        if (distance > shootingRange)
        {
            if (!isShooting)
            {
                float currentSpeed = distance <= boostRange ? boostSpeed : moveSpeed;

                Vector2 dir = (player.position - transform.position).normalized;
                Vector2 targetVelocity = new Vector2(dir.x * currentSpeed, rb.linearVelocity.y);

                rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, chaseAcceleration * Time.deltaTime);
            }
        }
        else // 玩家進入射擊距離
        {
            rb.linearVelocity = Vector2.zero;

            if (shootTimer >= shootInterval && !isShooting)
            {
                StartCoroutine(ShootSequence());
                shootTimer = 0f; // ✅ 重置計時器
            }
        }

        FlipToFacePlayer();
    }

    IEnumerator ShootSequence()
    {
        isShooting = true;

        FireBullet();

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

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootingRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, boostRange); // ✅ 顯示加速範圍
    }
#endif
}
