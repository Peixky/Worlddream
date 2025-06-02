using UnityEngine;

public class ZiplineRider : MonoBehaviour
{
    public float ziplineSpeed = 5f;
    public float jumpForce = 7f;

    private Zipline currentZipline;
    private bool onZipline = false;
    private Rigidbody2D rb;

    private float t = 0f; // 0=start, 1=end
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 direction;
    private float ziplineLength;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Zipline zipline = other.GetComponent<Zipline>();
        if (zipline != null && !onZipline)
        {
            AttachToZipline(zipline);
        }
    }

    public void AttachToZipline(Zipline zipline)
    {
        currentZipline = zipline;
        onZipline = true;

        // Get points
        startPoint = zipline.startPoint.position;
        endPoint = zipline.endPoint.position;
        direction = (endPoint - startPoint).normalized;
        ziplineLength = Vector2.Distance(startPoint, endPoint);

        // 計算目前接觸點在線段上的比例 t
        Vector2 closest = zipline.ClosestPoint(transform.position);
        float distFromStart = Vector2.Distance(startPoint, closest);
        t = distFromStart / ziplineLength;

        // 停用物理重力
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        // 馬上移動到線段上最近點
        transform.position = closest;
    }

    private void Update()
    {
        if (onZipline)
        {
            // 自動跳離（你可改成手動跳離）
            if (t >= 1f)
            {
                JumpOffZipline();
            }

            if (Input.GetKeyDown(KeyCode.Space)) // optional
            {
                JumpOffZipline();
            }
        }
    }

    private void FixedUpdate()
    {
        if (onZipline)
        {
            float delta = (ziplineSpeed / ziplineLength) * Time.fixedDeltaTime;
            t = Mathf.Clamp01(t + delta);

            Vector2 pos = Vector2.Lerp(startPoint, endPoint, t);
            transform.position = pos;

            // 確保速度與方向一致（可用於跳出用）
            rb.linearVelocity = direction * ziplineSpeed;
        }
    }
    private void JumpOffZipline()
    {
        onZipline = false;
        rb.gravityScale = 1f;

        // 往滑索方向瞬移一段距離
        float dashDistance = 0.5f; // 可調整為你想要的距離
        Vector2 dashOffset = direction * dashDistance;
        transform.position += (Vector3)dashOffset;

        // 重啟碰撞器防止馬上再掛回滑索
        if (currentZipline != null)
        {
            StartCoroutine(ResetZiplineCollider(currentZipline));
        }

        currentZipline = null;
    }


    private System.Collections.IEnumerator ResetZiplineCollider(Zipline zipline)
    {
        Collider2D ziplineCollider = zipline.GetComponent<Collider2D>();
        if (ziplineCollider != null)
        {
            ziplineCollider.enabled = false;

            int waitFrames = 30; // 等 20 幀
            for (int i = 0; i < waitFrames; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            ziplineCollider.enabled = true;
        }
    }




}
