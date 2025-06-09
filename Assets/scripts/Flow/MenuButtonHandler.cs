using UnityEngine;

public class MenuButtonHandler : MonoBehaviour
{
    public void StartGame()
    {
        if (GameProgressionManager.instance != null)
        {
            GameProgressionManager.StartGameFlow();
            Debug.Log("GameProgressionManager.StartGameFlow() 已呼叫！");
        }
        else
        {
            Debug.LogError("GameProgressionManager 尚未初始化！無法啟動遊戲流程。請檢查 GameProgressionManager 的載入順序或 GameObject 設定。", this);
            SceneFlowManager.Instance.nextSceneName = "Story/Scene/開場劇情";
            SceneFlowManager.Instance.GoToNextScene();
        }
        
    }
}