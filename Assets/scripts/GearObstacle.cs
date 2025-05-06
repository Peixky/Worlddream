using UnityEngine;

public class GearObstacle : MonoBehaviour
{
<<<<<<< HEAD
    public Transform player;        // 設定為 attack_01
    public float triggerDistance = 3f;
=======
    public Transform player;         // 設定為 attack_01
    public float triggerY = 2f;      // 玩家必須達到的 Y 高度（像地板上某一層）
    public float tolerance = 0.05f;  // 允許的高度誤差（愈小愈精準）
>>>>>>> 25a84ad (sss)
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

<<<<<<< HEAD
    float verticalDistance = Mathf.Abs(transform.position.y - player.position.y);
    if (verticalDistance <= triggerDistance)
{
    Fall();
}

=======
        float yDiff = Mathf.Abs(player.position.y - triggerY);
        if (yDiff <= tolerance)
        {
            Fall();
        }
>>>>>>> 25a84ad (sss)
    }

    void Fall()
    {
        hasFallen = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = new Vector2(0, fallSpeed); // 往下掉
    }
}