using UnityEngine;
using System.Collections;

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

        if (endRotationZ > startRotationZ && (endRotationZ - startRotationZ) > 180f)
            startRotationZ -= 360f;
        else if (startRotationZ > endRotationZ && (startRotationZ - endRotationZ) > 180f)
            endRotationZ -= 360f;

        float elapsedTime = 0f;
        float totalDegrees = Mathf.Abs(endRotationZ - startRotationZ);
        float duration = (rotationSpeed > 0) ? (totalDegrees / rotationSpeed) : 0;

        while (elapsedTime < duration)
        {
            float z = Mathf.Lerp(startRotationZ, endRotationZ, elapsedTime / duration);
            transform.rotation = Quaternion.Euler(0, 0, z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, targetRotationZ);
        if (GameProgressionManager.CurrentLevelIndex == (GameProgressionManager.instance.gameScenes.Length - 1)) 
        {
            Debug.Log("BossHealth: 最後一關 Boss 死亡。加載劇情四。");
            GameProgressionManager.AdvanceStory(); // 推進劇情到劇情四
            GameProgressionManager.LoadNextStoryScene(); // 加載 StoryScene4
        }
        else // 其他關卡的 Boss 死亡 (這個流程中，只有第三關是 Boss 死亡觸發劇情)
        {
            // 這裡根據流程圖，其他關卡的 Boss 死亡不會發生，或者會導致流程錯誤
            // 因為只有 Level3GameScene 的 Boss 死亡才會觸發劇情四。
            // 如果其他關卡也有 Boss 死亡的邏輯，而您希望它們回到大廳，則需要額外判斷
            Debug.LogWarning("BossHealth: 非最後一關的 Boss 死亡，但流程圖中未指定後續動作。");
            // GameProgressionManager.LoadLobbyScene(); // 例如可以回到大廳
        }
    }
}
