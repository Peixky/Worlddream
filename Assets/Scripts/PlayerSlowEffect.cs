using UnityEngine;
using System.Collections;

public class PlayerSlowEffect : MonoBehaviour
{
    public float slowMultiplier = 0.5f; 
    public float duration = 1f;

    private Rigidbody2D rb;
    private bool isSlowing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null && !isSlowing)
        {
            StartCoroutine(ApplySlow());
        }
        else
        {
            Destroy(this); 
        }
    }

    private IEnumerator ApplySlow()
    {
        isSlowing = true;

        float timer = 0f;
        while (timer < duration)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * slowMultiplier, rb.linearVelocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(this); // 結束後自動移除
    }
}