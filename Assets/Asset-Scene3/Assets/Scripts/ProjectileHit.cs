using UnityEngine;

public class ProjectileHit : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // <<<<<< 修改這裡：從 HealthSystem 改為 PlayerHealth >>>>>>
            PlayerHealth hp = other.GetComponent<PlayerHealth>(); 
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