using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime); // 防止炸彈卡住
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
       

        Destroy(gameObject); // 撞到任何東西就爆
    }
}
