using UnityEngine;
using System.Collections;

public class SpikeTrapController : MonoBehaviour
{
    public float riseHeight = 1f;
    public float riseDuration = 0.3f;
    public float delayBeforeRise = 0.1f;
    public float activeDuration = 1.5f;        // 地刺冒出後停留多久
    public float cooldownAfterReset = 1f;      // 收回後多快可以再觸發

    private Vector3 startPos;
    private Vector3 targetPos;
    private BoxCollider2D damageCollider;

    private bool isActive = false;
    private bool isOnCooldown = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * riseHeight;

        // 取得地刺圖層的碰撞器
        Transform visual = transform.Find("SpikeVisual");
        if (visual != null)
        {
            damageCollider = visual.GetComponent<BoxCollider2D>();
            if (damageCollider != null)
                damageCollider.enabled = false; // 一開始關掉傷害
        }
        else
        {
            Debug.LogError("找不到 SpikeVisual");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive || isOnCooldown) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(SpikeCycle());
        }
    }

    IEnumerator SpikeCycle()
    {
        isActive = true;
        yield return new WaitForSeconds(delayBeforeRise);

        // 上升
        float t = 0f;
        while (t < riseDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, t / riseDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;

        // 啟用傷害
        if (damageCollider != null)
            damageCollider.enabled = true;

        // 停留在上方一段時間
        yield return new WaitForSeconds(activeDuration);

        // 收回地刺
        t = 0f;
        while (t < riseDuration)
        {
            transform.position = Vector3.Lerp(targetPos, startPos, t / riseDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;

        // 關閉傷害
        if (damageCollider != null)
            damageCollider.enabled = false;

        // 冷卻時間
        isActive = false;
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownAfterReset);
        isOnCooldown = false;
    }
}
