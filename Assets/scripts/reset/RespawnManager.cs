using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private Transform currentRespawnPoint;
    private GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保留重載場景後仍在
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetRespawnPoint(Transform point)
    {
        currentRespawnPoint = point;
        Debug.Log("RespawnManager: 設定重生點 -> " + point.name);
    }

    public void Respawn(GameObject playerObj)
    {
        if (currentRespawnPoint == null)
        {
            Debug.LogWarning("RespawnManager: 尚未設定重生點！");
            return;
        }

        player = playerObj; // 保存目前玩家，供場景載入後使用

        // 重新載入場景（將會清空場景物件，但此物件會保留）
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // 嘗試找新玩家（場景重載後原本玩家可能不存在）
        var newPlayer = GameObject.FindWithTag("Player");

        if (newPlayer != null)
        {
            newPlayer.transform.position = currentRespawnPoint.position;
            newPlayer.transform.rotation = currentRespawnPoint.rotation;

            // ✅ 回復血量
            var health = newPlayer.GetComponent<Health>();
            if (health != null)
                health.SetHealth(health.MaxHealth);

            // ✅ 重設死亡旗標
            var damageHandler = newPlayer.GetComponent<PlayerDamageHandler>();
            if (damageHandler != null)
                damageHandler.ResetState();

            Debug.Log("RespawnManager: 玩家已重生在存檔點！");
        }
        else
        {
            Debug.LogWarning("RespawnManager: 找不到玩家物件！");
        }

        // ✅ 重置 IResettable 物件（可選）
        var resettableObjects = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IResettable>()
            .ToArray();

        foreach (var obj in resettableObjects)
        {
            obj.ResetState();
        }
    }
}
