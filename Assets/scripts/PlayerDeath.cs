using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathManager : MonoBehaviour
{
    private bool isKnockback = false;
    private float knockbackTime = 0.3f;
    private float knockbackTimer;

    [Header("掉出地圖會死亡")]
    public float deathYThreshold = -10f;

    [Header("死亡畫面 UI")]
    public GameObject deathScreenUI;

    private bool isDead = false;

    [Header("角色控制")]
    public Health health; // 連結 Health 腳本

    private void Start()
    {
        if (deathScreenUI != null)
            deathScreenUI.SetActive(false);
        else
            Debug.LogWarning("deathScreenUI 沒有設定！");
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
            health.TakeDamage(health.maxHealth); // 掉下地圖，扣光血量
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Spike"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = new Vector2(-8f, 15f);

            Playermovement pm = GetComponent<Playermovement>();
            if (pm != null)
            {
                pm.canMove = false;
                Invoke(nameof(EnableMovement), knockbackTime);
            }

            health.TakeDamage(1);
        }

        if (other.CompareTag("Enemy"))
        {
            health.TakeDamage(1);
        }

        if (other.CompareTag("KillZone"))
        {
            health.TakeDamage(health.maxHealth); // 碰到致命物件一次死亡
        }
    }

    private void EnableMovement()
    {
        Playermovement pm = GetComponent<Playermovement>();
        if (pm != null)
            pm.canMove = true;
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
