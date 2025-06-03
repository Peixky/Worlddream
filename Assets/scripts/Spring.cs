using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] private float bounceForce = 20f; // 彈跳力

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // 彈跳向上
                player.Bounce(new Vector2(0, bounceForce));
            }
        }
    }
}
