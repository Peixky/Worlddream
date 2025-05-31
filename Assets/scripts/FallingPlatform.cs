using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [Header("設定參數")]
    public float shakeDuration = 0.5f;     // 震動持續時間
    public float fallDelay = 0.5f;         // 從踩上到掉落延遲時間
    public float resetDelay = 3f;          // 多久後復原
    public float shakeAmount = 0.1f;       // 震動幅度

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody2D rb;
    private Collider2D col;

    private bool isTriggered = false;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isTriggered && collision.gameObject.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(HandlePlatform());
        }
    }

    IEnumerator HandlePlatform()
    {
        // 震動提示
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 randomPoint = originalPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
            transform.position = randomPoint;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition; // 還原位置（結束震動）

        yield return new WaitForSeconds(fallDelay);

        rb.bodyType = RigidbodyType2D.Dynamic; // 掉落
        col.enabled = false; // ✅ 關閉碰撞器（玩家穿透）

        yield return new WaitForSeconds(resetDelay);

        // 重設平台
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        col.enabled = true; // ✅ 恢復碰撞器
        isTriggered = false;
    }
}
