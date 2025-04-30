using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI 系統用

public class PlayerDeathManager : MonoBehaviour
{
    [Header("死亡設定")]
    public float deathYThreshold = -10f;  // 掉下地圖的高度
    public int maxHP = 3;                 // 最大血量
    private int currentHP;

    [Header("死亡畫面 UI")]
    public GameObject deathScreenUI;       // 黑色遮罩 Panel

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        deathScreenUI.SetActive(false); // 開場不要顯示
    }

    void Update()
    {
        if (isDead)
        {
            if (Input.anyKeyDown)
            {
                Time.timeScale = 1f; // 還原時間流動
                SceneManager.LoadScene("Scenes/Menu");
            }
            return;
        }

        // 判斷掉出地圖死亡
        if (transform.position.y < deathYThreshold)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // 碰到陷阱死亡
        if (other.CompareTag("Trap"))
        {
            Die();
        }

        // 碰到敵人扣血（你可以設敵人 Tag 為 "Enemy"）
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }

        // 撞到指定物件死亡（例如特殊機關）
        if (other.CompareTag("KillZone"))
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Time.timeScale = 0f;           // 暫停遊戲
        deathScreenUI.SetActive(true); // 顯示黑畫面
    }
}
