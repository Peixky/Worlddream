using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement; // 新增：用於獲取當前場景名稱

[RequireComponent(typeof(Health))]
public class PlayerDeathHandler : MonoBehaviour
{
    private Health health;

    // 可選：在 Inspector 中直接指定 Scene 3 的名稱，增加靈活性
    // 如果您在 GameProgressionManager 中已經有明確的場景名稱列表，可以不使用這個
    [SerializeField] private string scene3Name = "Level3GameScene"; // 預設值，請替換為您實際的 Scene 3 名稱

    private void Awake()
    {
        health = GetComponent<Health>();

        // 如果 scene3Name 在 Inspector 中未設定，則嘗試從 GameProgressionManager 獲取
        // 假設 GameProgressionManager.instance.gameScenes[2] 是 Scene 3 的名稱
        if (string.IsNullOrEmpty(scene3Name) && GameProgressionManager.instance != null && GameProgressionManager.instance.gameScenes.Length > 2)
        {
            scene3Name = GameProgressionManager.instance.gameScenes[2];
            Debug.Log($"PlayerDeathHandler: Scene 3 名稱從 GameProgressionManager 獲取為: {scene3Name}");
        }
        else if (string.IsNullOrEmpty(scene3Name))
        {
            Debug.LogWarning("PlayerDeathHandler: Scene 3 名稱未設定，也無法從 GameProgressionManager 獲取。請在 Inspector 中設置或檢查 GameProgressionManager。");
        }
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
        string currentSceneName = SceneManager.GetActiveScene().name; // 獲取當前場景名稱

        // 判斷是否為 Scene 3
        if (currentSceneName == scene3Name)
        {
            // ===== 這是 Scene 3: 顯示 Game Over 面板並進入劇情 =====
            Debug.Log($"玩家在 {currentSceneName} 死亡，觸發遊戲結束流程 (Scene 3)。");
            var manager = FindFirstObjectByType<IntroManager>();
            if (manager != null)
            {
                // IntroManager.ShowGameOver 會處理：
                // 1. Time.timeScale = 0f (暫停遊戲)
                // 2. GameProgressionManager.currentGameState = GameState.GameOver
                // 3. 顯示淡入效果和 "GAME OVER" 文字
                // 4. 最後呼叫 GameProgressionManager.LoadNextStoryScene() 載入劇情場景
                IntroManager.ShowGameOver();
            }
            else
            {
                Debug.LogError("PlayerDeathHandler: 找不到 IntroManager 實例，無法顯示 Scene 3 的 GameOver 畫面！" +
                               "請確認 IntroManager 實例存在於 Scene 3。", this);
                // 如果 IntroManager 意外丟失，作為備用方案，仍然暫停遊戲並載入劇情場景
                Time.timeScale = 0f;
                GameProgressionManager.currentGameState = GameProgressionManager.GameState.GameOver;
                GameProgressionManager.LoadNextStoryScene();
            }
            // 在 Scene 3 死亡時，不需要呼叫 RespawnManager，因為遊戲會直接結束並進入劇情。
        }
        else // ===== 這是 Scene 1 或 Scene 2: 直接重生並繼續玩 =====
        {
            Debug.Log($"玩家在 {currentSceneName} 死亡，直接重生並恢復遊戲。");
            
            // 呼叫重生管理器，讓玩家在當前場景重生
            RespawnManager.Instance.Respawn(gameObject); 

            // 確保遊戲時間和狀態在重生後恢復正常
            // 這部分最好由 RespawnManager 在重生完成後負責。
            // 但如果您的 RespawnManager 不包含這些，可以暫時放在這裡作為確保。
            Time.timeScale = 1f; 
            GameProgressionManager.currentGameState = GameProgressionManager.GameState.Playing; 

            // 如果玩家控制器在死亡時被禁用，這裡可能需要重新啟用它
            // 例如：GetComponent<PlayerController>()?.EnableMovement();
        }
    }
}