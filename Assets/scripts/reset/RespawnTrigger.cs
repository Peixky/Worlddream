using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) // 2D版，根據你遊戲設定
    {
        if (other.CompareTag("Player"))
        {
            RespawnManager.Instance.SetRespawnPoint(transform);
        }
    }

}
