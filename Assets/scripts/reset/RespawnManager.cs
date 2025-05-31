using UnityEngine;
using System.Linq;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private Transform currentRespawnPoint;
    private IResettable[] resettableObjects;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 找出場景中所有實作 IResettable 的物件
        resettableObjects = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IResettable>()
            .ToArray();
    }

    public void SetRespawnPoint(Transform point)
    {
        currentRespawnPoint = point;
    }

    public void Respawn(GameObject player)
    {
        if (currentRespawnPoint != null)
        {
            // 重設所有支援 Reset 的物件
            foreach (var obj in resettableObjects)
            {
                obj.ResetState();
            }

            // ✅ 1. 傳送玩家到重生點
            player.transform.position = currentRespawnPoint.position;

            // ✅ 2. 回滿血
            var health = player.GetComponent<Health>();
            if (health != null)
                health.SetHealth(health.MaxHealth);

            // ✅ 3. 解除死亡旗標（如 PlayerDamageHandler 有 isDead）
            var damageHandler = player.GetComponent<PlayerDamageHandler>();
            if (damageHandler != null)
                damageHandler.ResetState();
        }
        else
        {
            Debug.LogWarning("尚未設定重生點！");
        }
    }

}
