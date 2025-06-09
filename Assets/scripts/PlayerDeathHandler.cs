using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Health))]
public class PlayerDeathHandler : MonoBehaviour
{
    private Health health;

    [SerializeField] private string scene3Name = "Scene3";

    private void Awake()
    {
        health = GetComponent<Health>();

        
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
        string currentSceneName = SceneManager.GetActiveScene().name; 

        if (currentSceneName == scene3Name)
        {
            Debug.Log($"玩家在 {currentSceneName} 死亡，觸發遊戲結束流程 (Scene 3)。");
            var manager = FindFirstObjectByType<IntroManager>();
            if (manager != null)
            {

                IntroManager.ShowGameOver();
            }
            else
            {
                Debug.LogError("PlayerDeathHandler: 找不到 IntroManager 實例，無法顯示 Scene 3 的 GameOver 畫面！" +
                               "請確認 IntroManager 實例存在於 Scene 3。", this);

                Time.timeScale = 0f;
                GameProgressionManager.currentGameState = GameProgressionManager.GameState.GameOver;
                GameProgressionManager.LoadNextStoryScene();
            }
 
        }
        else 
        {
            Debug.Log($"玩家在 {currentSceneName} 死亡，直接重生並恢復遊戲。");
            
            
            RespawnManager.Instance.Respawn(gameObject); 


            Time.timeScale = 1f; 
            GameProgressionManager.currentGameState = GameProgressionManager.GameState.Playing; 

        }
    }
}