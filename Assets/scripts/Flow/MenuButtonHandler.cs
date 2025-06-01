using UnityEngine;
// using UnityEngine.SceneManagement; // 如果 SceneFlowManager 已經處理，這裡不需要

public class MenuButtonHandler : MonoBehaviour
{
    public void StartGame()
    {
        // 先呼叫 GameProgressionManager.StartGameFlow() 來初始化進度並開始遊戲流程
        // GameProgressionManager 內部會處理載入第一個劇情場景的邏輯 (LoadNextStoryScene)
        if (GameProgressionManager.instance != null) // 再次加上 null 檢查以防萬一
        {
            GameProgressionManager.StartGameFlow();
            Debug.Log("GameProgressionManager.StartGameFlow() 已呼叫！");
        }
        else
        {
            Debug.LogError("GameProgressionManager 尚未初始化！無法啟動遊戲流程。請檢查 GameProgressionManager 的載入順序或 GameObject 設定。", this);
            // 如果 GameProgressionManager 沒準備好，至少先切場景以防卡住
            SceneFlowManager.Instance.nextSceneName = "Story/Scene/開場劇情";
            SceneFlowManager.Instance.GoToNextScene();
        }
        // 注意：如果你讓 GameProgressionManager.StartGameFlow() 負責載入場景，
        // 那麼你可能不需要再調用 SceneFlowManager.Instance.GoToNextScene() 了。
        // 因為 GameProgressionManager.StartGameFlow() 最後會呼叫 LoadNextStoryScene()
        // 而 LoadNextStoryScene() 會負責 SceneManager.LoadScene()。
        // 所以下面的這一行可能會被移除，避免重複載入或邏輯衝突。
        // SceneFlowManager.Instance.nextSceneName = "Story/Scene/開場劇情"; // 這一行可能會被移除
        // SceneFlowManager.Instance.GoToNextScene(); // 這一行可能會被移除
    }
}