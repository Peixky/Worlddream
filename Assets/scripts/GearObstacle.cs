using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class GearObstacle : MonoBehaviour
{
    [Tooltip("Fall (32x32)")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("3.5")]
    [SerializeField] private float triggerDistance = 5f;

    [Tooltip("-8")]
    [SerializeField] private float fallSpeed = -5f;

    private Rigidbody2D rb;
    private bool hasTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 初始時凍結剛體，齒輪不會掉落
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (hasTriggered || playerTransform == null)
            return;

        // 計算玩家與齒輪的距離
        float dist = Vector2.Distance(
            transform.position,
            playerTransform.position
        );

        if (dist <= triggerDistance)
        {
            TriggerFall();
        }
    }

    private void TriggerFall()
    {
        hasTriggered = true;
        // 切換至動態剛體，並設置掉落速度
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = new Vector2(0f, fallSpeed);
    }

    // 在 Scene 視窗中繪制觸發範圍（紅色圓形）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}
