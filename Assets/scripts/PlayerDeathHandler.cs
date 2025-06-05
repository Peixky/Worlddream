using UnityEngine;
using UnityEngine.SceneManagement; // 需要這個命名空間來判斷當前場景名稱

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
        health.OnDied += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        health.OnDied -= HandlePlayerDeath;
    }

    // 當玩家死亡時呼叫此方法
    private void HandlePlayerDeath()
    {
        // 判斷當前場景是否為 Level3GameScene (第三關)
        // 假設 GameProgressionManager.instance.gameScenes[2] 是 Level3GameScene 的名稱
        bool isInLevel3 = false;
        if (GameProgressionManager.instance != null) // 確保 GameProgressionManager 實例存在
        {
            isInLevel3 = (SceneManager.GetActiveScene().name == GameProgressionManager.instance.gameScenes[2]);
        }
        else
        {
            // 如果 GameProgressionManager 不存在，無法判斷關卡。
            // 為了避免卡住或誤判，發出錯誤並假設為非 Level 3，讓玩家可以嘗試重生。
            Debug.LogError("PlayerDeathHandler: GameProgressionManager 實例未找到！無法判斷當前關卡，將預設為非 Level 3 行為。", this);
            isInLevel3 = false; // 如果找不到 GameProgressionManager，就預設不是 Level 3，讓它走重生邏輯
        }


        if (!isInLevel3) // 如果不是 Level 3 關卡，就執行重生邏輯
        {
            RespawnManager.Instance.Respawn(gameObject);
            Debug.Log("PlayerDeathHandler: 玩家在非 Level 3 關卡死亡，玩家重生。");
        }
        else // 如果是 Level 3 關卡，則不執行重生，只觸發遊戲結束畫面
        {
            Debug.Log("PlayerDeathHandler: 玩家在 Level 3 關卡死亡，不進行重生。");
            var introManagerInstance = FindFirstObjectByType<IntroManager>();
            if (introManagerInstance != null)
            {
                // IntroManager.ShowGameOver() 內部會再次判斷是否為 Level 3 才顯示 UI，
                // 並且在非 Level 3 時，只會暫停遊戲時間而不顯示 UI。
                IntroManager.ShowGameOver(); 
                Debug.Log("PlayerDeathHandler: 觸發 IntroManager 遊戲結束流程 (是否顯示 UI 依關卡而定)。");
            }
            else
            {
                // 這個錯誤應該只在 IntroManager GameObject 不存在於場景中時出現，
                // 但如果它存在，只是沒有在 Level 3，ShowGameOver 也會處理
                Debug.LogError("PlayerDeathHandler: 找不到 IntroManager 實例，無法觸發 GameOver 畫面！", this);
            }
        }


        // 呼叫 IntroManager 來顯示遊戲結束畫面 (IntroManager 內部會自己判斷是否在 Level 3 顯示 UI)
        
    }
}