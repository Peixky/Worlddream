using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireInterval = 2f;
    public float bulletSpeed = 5f;

    // 可在 Inspector 設定子彈方向（角度，0度為右，逆時針）
    public float fireAngle = 0f;

    // 方向供反彈使用：1 = 向右, -1 = 向左
    public int bulletDirection = 1;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= fireInterval)
        {
            Fire();
            timer = 0f;
        }
    }

    private void Fire()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 設定方向
        float rad = fireAngle * Mathf.Deg2Rad;
        Vector2 velocity = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * bulletSpeed;

        Rigidbody2D rb = bulletObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }

        // 設定方向資料供 PlayerDeath 使用
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.direction = bulletDirection;
        }

        // 選配：讓子彈朝著移動方向旋轉（可視化）
        bulletObj.transform.rotation = Quaternion.Euler(0, 0, fireAngle);
    }
}
