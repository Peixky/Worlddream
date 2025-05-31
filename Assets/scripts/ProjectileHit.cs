using UnityEngine;

public class ProjectileHit : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ✅ 改為通用 Health.cs，支援玩家或其他角色
            Health hp = other.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger) // 撞到非Trigger物件就刪除自己
        {
            Destroy(gameObject); 
        }
    }
}
