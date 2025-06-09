using UnityEngine;

public class ProjectileHit : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health hp = other.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject); 
        }
    }
}
