using UnityEngine;

public class EnemyEdgeWallDetector : MonoBehaviour
{
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;

    public float checkDistance = 0.1f;
    public bool stopAtEdge = true;
    public bool stopAtWall = true;

    public bool IsNearEdge { get; private set; }
    public bool IsNearWall { get; private set; }

    void Update()
    {
        if (groundCheck != null)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);
            IsNearEdge = groundHit.collider == null;
        }

        if (wallCheck != null)
        {
            RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, transform.right, checkDistance, groundLayer);
            IsNearWall = wallHit.collider != null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawRay(groundCheck.position, Vector2.down * checkDistance);

        if (wallCheck != null)
            Gizmos.DrawRay(wallCheck.position, transform.right * checkDistance);
    }
}
