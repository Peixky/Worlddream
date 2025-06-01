using UnityEngine;
// using UnityEngine.SceneManagement; // SceneFlowManager 或 GameProgressionManager 會處理場景載入

public class LevelExitPoint : MonoBehaviour
{
    [Header("觸發設定")]
    public string playerTag = "Player"; // 確保你的玩家 GameObject 有這個 Tag
    public float transitionDelay = 1.0f; // 延遲幾秒後切場景

    [Header("結束後目標")]
    // 這個枚舉讓您在 Inspector 中選擇觸發後要執行的動作
    public ExitTarget target = ExitTarget.NextStory; // 預設為進入下一劇情
    public enum ExitTarget
    {
        NextStory,      // 觸發後推進劇情索引，然後載入下一段劇情 (例如 Scene1 -> 劇情二)
        Lobby,          // 觸發後直接載入大廳 (例如 level2 -> Lobby)
        // NextGameLevel // 如果未來有需求，可以新增這個選項：推進關卡索引，然後載入下一關遊戲場景
    }

    private bool playerReached = false; // 防止重複觸發

    // 當 3D Collider 觸發時呼叫
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !playerReached)
        {
            playerReached = true; // 設定為已觸發
            Debug.Log($"LevelExitPoint: 玩家觸發出口點 ({gameObject.name})。目標: {target}。");
            Invoke("TriggerTransition", transitionDelay); // 延遲後執行轉換邏輯
        }
    }

    // 當 2D Collider 觸發時呼叫
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !playerReached)
        {
            playerReached = true; // 設定為已觸發
            Debug.Log($"LevelExitPoint: 玩家觸發出口點 ({gameObject.name})。目標: {target}。");
            Invoke("TriggerTransition", transitionDelay); // 延遲後執行轉換邏輯
        }
    }

    void TriggerTransition()
    {
        if (GameProgressionManager.instance == null)
        {
            Debug.LogError("LevelExitPoint: GameProgressionManager 尚未初始化！無法執行場景轉換。", this);
            // 作為備用方案，如果 GameProgressionManager 沒準備好，直接切換到預設下一個場景
            // 但這會繞過進度管理，通常不建議作為最終解決方案，僅用於開發階段的緊急情況。
            // SceneFlowManager.Instance.nextSceneName = "FallbackSceneName"; // 你可以設定一個預設的錯誤場景
            // SceneFlowManager.Instance.GoToNextScene();
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
            // case ExitTarget.NextGameLevel:
            //     Debug.Log("LevelExitPoint: 呼叫 GameProgressionManager.AdvanceLevel() & LoadNextGameScene()。");
            //     GameProgressionManager.AdvanceLevel();
            //     GameProgressionManager.LoadNextGameScene();
            //     break;
        }
    }
}