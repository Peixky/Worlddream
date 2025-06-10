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

        // 讓該物件也不會被場景刪除
        if (point != null)
        {
            DontDestroyOnLoad(point.gameObject);
        }

        Debug.Log("RespawnManager: 設定重生點 -> " + point.name);
    }

    public void Respawn(GameObject playerObj)
    {
        if (currentRespawnPoint == null || currentRespawnPoint.Equals(null))
        {
            Debug.LogWarning("RespawnManager: 尚未設定有效的重生點！");
            return;
        }

        player = playerObj; 

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // 嘗試找新玩家（場景重載後原本玩家可能不存在）
        var newPlayer = GameObject.FindWithTag("Player");

        if (newPlayer != null && currentRespawnPoint != null && !currentRespawnPoint.Equals(null))
        {
            newPlayer.transform.position = currentRespawnPoint.position;
            newPlayer.transform.rotation = currentRespawnPoint.rotation;

            var health = newPlayer.GetComponent<Health>();
            if (health != null)
                health.SetHealth(health.MaxHealth);

            var damageHandler = newPlayer.GetComponent<PlayerDamageHandler>();
            if (damageHandler != null)
                damageHandler.ResetState();

            Debug.Log("RespawnManager: 玩家已重生在存檔點！");
        }
        else
        {
            Debug.LogWarning("RespawnManager: 找不到玩家或重生點無效！");
        }

        // 重置所有 IResettable 物件
        var resettableObjects = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IResettable>()
            .ToArray();

        foreach (var obj in resettableObjects)
        {
            obj.ResetState();
        }
    }
}
