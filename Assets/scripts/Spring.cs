using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] private float bounceForce = 20f; // 彈跳力

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // 彈跳向上
                player.Bounce(new Vector2(0, bounceForce));
            }
        }
    }
}
