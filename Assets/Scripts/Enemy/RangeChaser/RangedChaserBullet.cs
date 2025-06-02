using UnityEngine;

public class RangedChaserBullet : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime); // 時間到自動刪除
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
