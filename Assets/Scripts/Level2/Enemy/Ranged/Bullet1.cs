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
            if (other.GetComponent<PlayerSlowEffect>() == null)
            {
                PlayerSlowEffect effect = other.gameObject.AddComponent<PlayerSlowEffect>();
                effect.slowMultiplier = 0.25f;
                effect.duration = 1f;         
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
