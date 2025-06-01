using UnityEngine;
using UnityEngine.SceneManagement; // <<<< 確保有這個命名空間 >>>>>

public class LevelExitPoint : MonoBehaviour
{
    [Header("觸發設定")]
    public string playerTag = "Player"; // 確保你的玩家 GameObject 有這個 Tag
    public float transitionDelay = 1.0f; // 延遲幾秒後切場景 (例如 Player 播放離開動畫的時間)

    [Header("結束後目標")]
    // 這個枚舉讓您在 Inspector 中選擇觸發後要執行的動作
    public ExitTarget target = ExitTarget.StoreScene; // 預設為進入商店場景
    public enum ExitTarget
    {
        NextStory,      // 觸發後推進劇情索引，然後載入下一段劇情
        Lobby,          // 觸發後直接載入大廳
        StoreScene,     // 觸發後直接載入商店場景
        // 如果未來有直接進入下一關的需求，可以新增 NextGameLevel
        // NextGameLevel
    }

    private bool playerReached = false; // 防止重複觸發

    // Unity 內建方法，當 3D Collider 觸發時呼叫 (如果 Player 是 3D)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !playerReached)
        {
            playerReached = true; 
            Debug.Log($"LevelExitPoint: 玩家觸發出口點 ({gameObject.name})。目標: {target}。");
            Invoke("TriggerTransition", transitionDelay); 
        }
    }

    // Unity 內建方法，當 2D Collider 觸發時呼叫 (如果 Player 是 2D，我們使用的是這個)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !playerReached)
        {
            playerReached = true; 
            Debug.Log($"LevelExitPoint: 玩家觸發出口點 ({gameObject.name})。目標: {target}。");
            Invoke("TriggerTransition", transitionDelay); 
        }
    }

    void TriggerTransition()
    {
        // 確保 GameProgressionManager 已經被初始化 (例如在 MainMenuScene 裡)
        if (GameProgressionManager.instance == null)
        {
            Debug.LogError("LevelExitPoint: GameProgressionManager 尚未初始化！無法執行場景轉換。請確保 GameManager 物件存在於遊戲啟動場景。", this);
            // 作為備用方案，如果 GameProgressionManager 沒準備好，直接切換到主選單或特定錯誤場景
            SceneManager.LoadScene(0); // 這裡假設 MainMenuScene 是索引 0
            return;
        }

        switch (target)
        {
            case ExitTarget.NextStory:
                Debug.Log("LevelExitPoint: 呼叫 GameProgressionManager.AdvanceStory() & LoadNextStoryScene()。");
                GameProgressionManager.AdvanceStory();      // 推進劇情索引
                GameProgressionManager.LoadNextStoryScene(); // 載入新的 CurrentStoryIndex 對應的劇情
                break;
            case ExitTarget.Lobby:
                Debug.Log("LevelExitPoint: 呼叫 GameProgressionManager.LoadLobbyScene()。");
                GameProgressionManager.LoadLobbyScene();    // 直接載入大廳
                break;
            case ExitTarget.StoreScene: // <<<< 新增的處理邏輯 >>>>
                Debug.Log("LevelExitPoint: 呼叫 GameProgressionManager.LoadStoreScene()。");
                // 這裡我們不推進 AdvanceLevel 或 AdvanceStory，因為商店是一個中轉站
                // 進度和劇情推進會在商店點擊「下一關」按鈕時發生
                GameProgressionManager.LoadStoreScene(); // 直接載入商店場景
                break;
            // case ExitTarget.NextGameLevel: // 如果未來有需求
            //     Debug.Log("LevelExitPoint: 呼叫 GameProgressionManager.AdvanceLevel() & LoadNextGameScene()。");
            //     GameProgressionManager.AdvanceLevel();
            //     GameProgressionManager.LoadNextGameScene();
            //     break;
        }
    }
}