using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ✅ 傷害處理
            

            // ✅ 加上減速效果（如果還沒加過）
            if (other.GetComponent<PlayerSlowEffect>() == null)
            {
                PlayerSlowEffect effect = other.gameObject.AddComponent<PlayerSlowEffect>();
                effect.slowMultiplier = 0.5f; // 50% 移動速度
                effect.duration = 1f;         // 持續 1 秒
            }

            Destroy(gameObject);
        }

        // ✅ 如果不是打到敵人，也銷毀
        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
