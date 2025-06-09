using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1ExitPoint : MonoBehaviour
{
    [Header("觸發設定")]
    public string playerTag = "Player"; 
    public float transitionDelay = 1.0f; 

    [Header("結束後目標")]
    public ExitTarget target = ExitTarget.StoreScene; // 預設為進入商店場景
    public enum ExitTarget
    {
        NextStory,      
        Lobby,          
        StoreScene,    
    }

    private bool playerReached = false; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !playerReached)
        {
            playerReached = true; 
            Debug.Log($"LevelExitPoint: 玩家觸發出口點 ({gameObject.name})。目標: {target}。");
            Invoke("TriggerTransition", transitionDelay); 
        }
    }

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
        if (GameProgressionManager.instance == null)
        {
            Debug.LogError("LevelExitPoint: GameProgressionManager 尚未初始化！無法執行場景轉換。請確保 GameManager 物件存在於遊戲啟動場景。", this);
            SceneManager.LoadScene(0); 
            return;
        }

        switch (target)
        {
            case ExitTarget.NextStory:
                Debug.Log("LevelExitPoint: 呼叫 GameProgressionManager.AdvanceStory() & LoadNextStoryScene()。");
                GameProgressionManager.AdvanceStory(); 
                GameProgressionManager.LoadNextStoryScene(); 
                break;
            case ExitTarget.Lobby:
                Debug.Log("LevelExitPoint: 呼叫 GameProgressionManager.LoadLobbyScene()。");
                GameProgressionManager.LoadLobbyScene(); 
                break;
            case ExitTarget.StoreScene: 
                Debug.Log("LevelExitPoint: 呼叫 GameProgressionManager.LoadStoreScene()。");
                GameProgressionManager.LoadStoreScene(); 
                break;
        }
    }
}