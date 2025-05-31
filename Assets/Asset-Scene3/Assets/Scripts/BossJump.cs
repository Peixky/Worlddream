using UnityEngine;

public class BossJump : MonoBehaviour
{
    public float jumpForce = 20f;
    public float minInterval = 0f;
    public float maxInterval = 10f;
    public CameraShake cameraShake;

    public float bounceForce = 5f; // 玩家彈起來的力道
    public float bounceRadius = 5f; // 彈起來的範圍半徑

    private Rigidbody2D rb;
    private bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(JumpRoutine());
    }

    System.Collections.IEnumerator JumpRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(2f, 4f); // 測試階段先改短

            yield return new WaitForSeconds(waitTime);

            Jump();
        }
    }

    void Jump()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
        }

        Debug.Log("Boss 跳起來了！");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isJumping && collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;

            // 畫面震動
            if (cameraShake != null)
            {
                StartCoroutine(cameraShake.Shake(0.3f, 0.4f));
            }

            // 找到玩家並彈起來
            BounceNearbyPlayers();

            Debug.Log("Boss 落地並震動！");
        }
    }

    void BounceNearbyPlayers()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, bounceRadius);

        foreach (Collider2D col in hits)
        {
            if (col.CompareTag("Player"))
            {
                Rigidbody2D playerRb = col.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // 只給垂直向上彈的力，不要改變水平速度
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
                    Debug.Log("玩家被 Boss 彈起！");
                }
            }
        }
    }

    // 顯示彈跳範圍（Editor 輔助）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bounceRadius);
    }
}
