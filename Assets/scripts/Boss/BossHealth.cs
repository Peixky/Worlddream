using UnityEngine;
using System.Collections; // 確保有這個命名空間，用於協程
using System; // 確保有這個命名空間，用於 Action 事件 (給 Lambda 用)

[RequireComponent(typeof(Health))]
public class BossHealth : MonoBehaviour
{
    [Header("Boss 死亡設定")]
    public float rotationSpeed = 100f;
    public float targetRotationZ = -90f;
    public float destroyDelay = 3f;

    private Rigidbody2D rb;
    private BossController bossController;
    private Health health;

    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bossController = GetComponent<BossController>();
        health = GetComponent<Health>();

        // 訂閱死亡事件
        health.OnDied += HandleDeath;
    }

    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " 死亡了！");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (bossController != null)
        {
            bossController.enabled = false;
            bossController.StopAllCoroutines();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        StartCoroutine(RotateToTarget());
        
    }

    IEnumerator RotateToTarget()
    {
        float currentRotationZ = transform.rotation.eulerAngles.z;
        float startRotationZ = currentRotationZ;
        float endRotationZ = targetRotationZ;

        // 計算旋轉方向，確保短路徑
        if (endRotationZ > startRotationZ && (endRotationZ - startRotationZ) > 180f)
            startRotationZ -= 360f;
        else if (startRotationZ < endRotationZ && (startRotationZ - endRotationZ) < -180f) // 修正這裡的邏輯
            startRotationZ += 360f;
        
        // 確保目標旋轉值在 0 到 360 之間，與 EulerAngles 一致
        if (targetRotationZ < 0) targetRotationZ += 360f;
        if (targetRotationZ >= 360) targetRotationZ -= 360f;


        float elapsedTime = 0f;
        // 如果 targetRotationZ 和當前旋轉非常接近，直接設定時間為 0 或避免除以零
        float totalDegrees = Mathf.Abs(endRotationZ - startRotationZ);
        float duration = (rotationSpeed > 0 && totalDegrees > 0.01f) ? (totalDegrees / rotationSpeed) : 0f;


        while (elapsedTime < duration)
        {
            float z = Mathf.Lerp(startRotationZ, endRotationZ, elapsedTime / duration);
            transform.rotation = Quaternion.Euler(0, 0, z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, targetRotationZ); // 確保最終角度精確

        // === 修正開始 ===
        Debug.Log("BossHealth: Boss倒下動畫結束。準備播放影片過場動畫。");
        
        // 直接使用 VideoCutsceneManager.Instance 存取實例
        if (VideoCutsceneManager.Instance != null)
        {
            // 播放影片，並在影片結束後觸發載入劇情四
            VideoCutsceneManager.Instance.PlayVideo(() => { // <<<< 修正這裡，使用 .Instance >>>>
                Debug.Log("BossHealth: 影片播放結束，加載劇情四。");
                GameProgressionManager.AdvanceStory(); // 推進劇情到劇情四
                GameProgressionManager.LoadPlayerDeathStoryScene(); // 加載 StoryScene4
            });
        }
        else
        {
            GameProgressionManager.AdvanceStory(); // 推進劇情
            GameProgressionManager.LoadNextStoryScene(); // 加載 StoryScene4
        }
    }
}