// BossHealth.cs
using UnityEngine;
using System.Collections;
using System; 

[RequireComponent(typeof(Rigidbody2D), typeof(Health))]
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

        health.OnDied += HandleDeath;
    }

    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " 死亡了！");

        if (ScreenFlashManager.Instance != null)
        {
            ScreenFlashManager.Instance.StopFlashing();
        }

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

        if (endRotationZ > startRotationZ && (endRotationZ - startRotationZ) > 180f)
            startRotationZ -= 360f;
        else if (startRotationZ < endRotationZ && (startRotationZ - endRotationZ) < -180f)
            startRotationZ += 360f;

        if (targetRotationZ < 0) targetRotationZ += 360f;
        if (targetRotationZ >= 360) targetRotationZ -= 360f;

        float elapsedTime = 0f;
        float totalDegrees = Mathf.Abs(endRotationZ - startRotationZ);
        float duration = (rotationSpeed > 0 && totalDegrees > 0.01f) ? (totalDegrees / rotationSpeed) : 0f;

        while (elapsedTime < duration)
        {
            float z = Mathf.Lerp(startRotationZ, endRotationZ, elapsedTime / duration);
            transform.rotation = Quaternion.Euler(0, 0, z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, targetRotationZ);

        Debug.Log("BossHealth: Boss倒下動畫結束。準備播放影片過場動畫。");

        if (VideoCutsceneManager.Instance != null)
        {
            // 播放影片，並在影片結束後觸發載入「Boss 死亡劇情 Scene」
            VideoCutsceneManager.Instance.PlayVideo(() => {
                Debug.Log("BossHealth: 影片播放結束，加載 Boss 死亡劇情 Scene。");
                GameProgressionManager.LoadBossDeathScene(); // <<<< 修正這裡，呼叫 LoadBossDeathScene >>>>
            });
        }
        else
        {
            Debug.LogError("BossHealth: 未找到 VideoCutsceneManager 實例！直接加載 Boss 死亡劇情 Scene。", this);
            GameProgressionManager.LoadBossDeathScene(); // <<<< 修正這裡 >>>>
        }
    }
}