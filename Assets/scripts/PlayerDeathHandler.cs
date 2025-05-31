using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerDeathHandler : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDied += RespawnPlayer;
    }

    private void OnDisable()
    {
        health.OnDied -= RespawnPlayer;
    }

    private void RespawnPlayer()
    {
        RespawnManager.Instance.Respawn(gameObject);
        Debug.Log("玩家死亡，重新重生");
    }
}
