using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    public float lifeTime = 3f;
    public LayerMask playerLayer;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            PlayerDamageHandler damageHandler = collision.GetComponent<PlayerDamageHandler>();
            if (damageHandler != null && !damageHandler.IsInvincible())
            {
                float direction = transform.position.x > collision.transform.position.x ? -1f : 1f;
                damageHandler.TakeHitFromShockwave(damage, direction);
            }
        }
    }
}
