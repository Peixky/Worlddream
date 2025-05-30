using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerDeathManager : MonoBehaviour
{
    private bool isKnockback = false;
    private float knockbackTime = 0.3f;
    private float knockbackTimer;

    [Header("反彈設定")]
    [SerializeField] private float knockbackHorizontalForce = 8f;
    [SerializeField] private float knockbackVerticalForce = 15f;

    [Header("掉出地圖會死亡")]
    public float deathYThreshold = -10f;

    [Header("死亡畫面 UI")]
    public GameObject deathScreenUI;

    private bool isDead = false;

    [Header("角色控制")]
    public Health health; // 連結 Health 腳本

    [Header("無敵設定")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float flashInterval = 0.1f;
    private bool isInvincible = false;

    private SpriteRenderer spriteRenderer;
    private bool hasTakenDamageRecently = false;

    private void Start()
    {
        if (deathScreenUI != null)
            deathScreenUI.SetActive(false);
        else
            Debug.LogWarning("deathScreenUI 沒有設定！");

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Player 上缺少 SpriteRenderer 元件！");
        }
    }

    private void Update()
    {
        if (isDead)
        {
            if (Input.anyKeyDown)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("Menu");
            }
            return;
        }

        if (transform.position.y < deathYThreshold)
        {
            TryTakeDamage(health.maxHealth); // 不需反彈
        }

        if (isKnockback)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockback = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("EnemyBody")) return;

        TryTakeDamage(1, null, collision);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("EnemyBullet")) return;

        float xDir = 1f;

        // 從子彈腳本拿方向資訊
        Bullet bulletScript = other.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            xDir = bulletScript.direction > 0 ? 1f : -1f; // 子彈向右飛 → 玩家向左彈
        }

        TryTakeDamage(1, xDir);

        Destroy(other.gameObject);
    }
    private void TryTakeDamage(int amount, float? xDirectionOverride = null, Collision2D collision = null)
    {
        if (hasTakenDamageRecently || isDead || isInvincible) return;

        hasTakenDamageRecently = true;
        TakeDamage(amount, xDirectionOverride, collision);
        Invoke(nameof(ResetDamageFlag), 0.05f);
    }

    private void ResetDamageFlag()
    {
        hasTakenDamageRecently = false;
    }

    private void TakeDamage(int amount, float? xDirectionOverride = null, Collision2D collision = null)
    {
        health.TakeDamage(amount);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float xDirection = xDirectionOverride ?? 1f;

            if (!xDirectionOverride.HasValue && collision != null)
            {
                ContactPoint2D contact = collision.GetContact(0);
                xDirection = transform.position.x < contact.point.x ? -1f : 1f;
            }

            Vector2 knockback = new Vector2(xDirection * knockbackHorizontalForce, knockbackVerticalForce);
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockback, ForceMode2D.Impulse);

            Debug.Log("反彈方向：" + knockback);
        }

        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.canMove = false;
            Invoke(nameof(EnableMovement), knockbackTime);
        }

        isKnockback = true;
        knockbackTimer = knockbackTime;

        StartCoroutine(InvincibilityCoroutine());

        if (health.currentHealth <= 0)
        {
            TriggerDeath();
        }
    }

    private void EnableMovement()
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null)
            pm.canMove = true;
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
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

    public void TriggerDeath()
    {
        isDead = true;
        Time.timeScale = 0f;

        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }
    }
}
