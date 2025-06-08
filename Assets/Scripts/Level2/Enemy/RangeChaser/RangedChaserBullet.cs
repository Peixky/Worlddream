using UnityEngine;

public class RangedChaserBullet : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 5f;

    [Header("旋轉設定")]
    public float rotationSpeed = 360f; // 每秒旋轉角度

    void Start()
    {
        Destroy(gameObject, lifetime); // 時間到自動刪除
    }

    void Update()
    {
        // 讓子彈持續旋轉
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // 撞到玩家後刪除
        }
        else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject); // 撞到牆壁等其他物件也刪除
        }
    }
}
