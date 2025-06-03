using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MonsterAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public LayerMask wallLayer;

    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.left;
    private bool isStopped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isStopped) return;

        Vector2 origin = (Vector2)transform.position + moveDirection * 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, 0.6f, wallLayer);

        if (hit.collider != null)
        {
            isStopped = true;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }

        Debug.DrawRay(origin, moveDirection * 0.6f, Color.red);

    }
}
