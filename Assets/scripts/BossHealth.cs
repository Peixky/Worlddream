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
    }
}
