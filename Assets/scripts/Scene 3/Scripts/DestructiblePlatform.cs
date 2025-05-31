using UnityEngine;
using System.Collections;

public class DestructiblePlatform : MonoBehaviour
{
    [Header("平台狀態")]
    public float shatterDelay = 3f; // <<<< 震動持續時間，也是震動後到掉落的延遲時間 >>>>
    public float restoreDelay = 5f;  
    public float dropSpeed = 10f;    
    public float dropDistance = 10f; 

    public float shakeMagnitude = 0.1f; // 震動幅度

    private Vector3 originalPosition; 
    private Collider2D platformCollider; 
    private SpriteRenderer spriteRenderer; 
    private bool isShattering = false; 

    void Awake()
    {
        originalPosition = transform.position; 
        platformCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartShatter()
    {
        if (isShattering) return; 
        isShattering = true;

        //Debug.Log(gameObject.name + " 的 StartShatter 被呼叫！正在啟動 ShatterRoutine。");
        StartCoroutine(ShatterRoutine());
    }

    IEnumerator ShatterRoutine()
    {
        //Debug.Log(gameObject.name + " ShatterRoutine 協程開始執行，進入震動階段。"); 

        // <<<<<< 震動效果程式碼 - 請確認這裡的程式碼是否正確存在 >>>>>>
        Vector3 initialLocalPosition = transform.localPosition; // 記錄原始相對位置

        float elapsedShakeTime = 0f;
        // 震動將持續 shatterDelay 秒
        while (elapsedShakeTime < shatterDelay) // 使用 shatterDelay 作為震動持續時間
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localPosition = initialLocalPosition + new Vector3(x, y, 0f);

            elapsedShakeTime += Time.deltaTime;
            yield return null; // 等待下一幀，讓震動效果在多幀中連續播放
        }
        transform.localPosition = initialLocalPosition; // 震動結束後回到原始位置
        yield return new WaitForSeconds(0.1f); // 震動結束後稍微延遲一下，再掉落 (可選，讓震動有結束感)
        // <<<<<< 震動效果程式碼結束 >>>>>>

        // 2. 掉落階段
        //Debug.Log(gameObject.name + " 震動延遲結束，開始掉落..."); // 這個 Debug Log 應該會在震動後才出現
        platformCollider.enabled = false; // 禁用碰撞體
        // spriteRenderer.enabled = false; // 如果要看掉落過程，不要在這裡隱藏

        Vector3 targetDropPosition = originalPosition - new Vector3(0, dropDistance, 0); 

        float startTime = Time.time;
        float journeyLength = Vector3.Distance(transform.position, targetDropPosition);
        float speed = dropSpeed;

        while (Vector3.Distance(transform.position, targetDropPosition) > 0.1f)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(transform.position, targetDropPosition, fractionOfJourney);
            yield return null;
        }

        transform.position = targetDropPosition; // 確保到達最終位置
        spriteRenderer.enabled = false; // 掉下去後再隱藏

        // 3. 恢復階段
        //Debug.Log(gameObject.name + " 等待恢復...");
        yield return new WaitForSeconds(restoreDelay); 

        //Debug.Log(gameObject.name + " 正在恢復...");
        transform.position = originalPosition; 
        spriteRenderer.enabled = true; 
        platformCollider.enabled = true; 
        isShattering = false; 
    }
}