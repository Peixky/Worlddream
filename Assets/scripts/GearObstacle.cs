using UnityEngine;

public class GearObstacle : MonoBehaviour
{
    public Transform player;        // 設定為 attack_01
    public float triggerDistance = 3f;
    public float fallSpeed = -8f;

    private Rigidbody2D rb;
    private bool hasFallen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // 一開始不要掉下來
    }

    void Update()
    {
        if (hasFallen || player == null) return;

    float verticalDistance = Mathf.Abs(transform.position.y - player.position.y);
    if (verticalDistance <= triggerDistance)
{
    Fall();
}

    }

    void Fall()
    {
        hasFallen = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = new Vector2(0, fallSpeed); // 往下掉
    }
}