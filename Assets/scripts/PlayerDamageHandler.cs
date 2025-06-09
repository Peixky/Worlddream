using UnityEngine;
using System.Collections; // 為了 Coroutine
using System; // 為了 Action (如果之前有用到)

[RequireComponent(typeof(Rigidbody2D), typeof(Health))]
public class PlayerDamageHandler : MonoBehaviour
{
    [Header("碰撞反應")]
    [SerializeField] private float knockbackForceX = 8f;
    [SerializeField] private float knockbackForceY = 15f;
    [SerializeField] private float knockbackDuration = 0.3f;

    [Header("掉落死亡高度")]
    public float deathYThreshold = -10f;

    [Header("無敵設定")]
    [SerializeField] private float invincibleTime = 1.5f;
    [SerializeField] private float flashInterval = 0.1f;

    [Header("擊退拳頭力道 (KnockbackProjectile)")]
    [SerializeField] private float shockKnockbackForceX = 12f;
    [SerializeField] private float shockKnockbackForceY = 10f;


    private bool isInvincible = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Health health;
    private PlayerController playerController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();

        playerController = GetComponent<PlayerController>(); 
    }

    private void Update()
    {
        if (transform.position.y < deathYThreshold && !isInvincible)
        {
            health.TakeDamage(health.MaxHealth);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBody") || collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 dir = collision.GetContact(0).point - (Vector2)transform.position;
            TakeHit(1, dir.x > 0 ? -1 : 1);
            
            Debug.Log("玩家碰到怪物");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            float xDir = bullet != null && bullet.direction < 0 ? 1f : -1f;
            TakeHit(1, xDir);
            Destroy(other.gameObject);
        }
    }

    private void TakeHit(int damage, float xDirection)
    {
        if (isInvincible || health.CurrentHealth <= 0) return;

        health.TakeDamage(damage);

        Vector2 knockback = new Vector2(xDirection * knockbackForceX, knockbackForceY);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        StartCoroutine(InvincibilityCoroutine());

        if (playerController != null)
        {
            playerController.canMove = false; 
            playerController.StartRecoilToLastIdle(knockbackDuration);
            playerController.StartKnockbackIgnoreCollision(knockbackDuration); 
        }

    }


    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibleTime)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        isInvincible = false;
    }
    public void ResetState()
    {
        isInvincible = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        var movement = GetComponent<PlayerController>();
        if (movement != null)
            movement.canMove = true;

        Debug.Log("玩家狀態已重置");
    }
    public void TakeHitFromShockwave(int damage, float direction)
    {
        TakeHit(damage, direction);
    }
    public bool IsInvincible()
    {
        return isInvincible;
    }
    public void ApplyKnockbackOnly(float xDirection)
    {
        Vector2 knockback = new Vector2(xDirection * shockKnockbackForceX, shockKnockbackForceY);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        if (playerController != null)
        {
            playerController.canMove = false;
            playerController.StartKnockbackIgnoreCollision(knockbackDuration);
        }


        Debug.Log("⚡ 玩家被 KnockbackProjectile 擊退（無傷害）");
    }
}