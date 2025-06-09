using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Button startGameButton; 

    void Start()
    {
        
        Time.timeScale = 1f;

        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameButtonClick);
        }
        else
        {
            Debug.LogError("MainMenuController: StartGameButton 未連結！請在 Inspector 中設定。", this);
        }
        if (FindObjectOfType<GameProgressionManager>() == null)
        {
            Debug.LogWarning("MainMenuController: GameProgressionManager 未找到，請確保 GameManager 物件在 MainMenuScene 中。");
        }
    }

    
    public void OnStartGameButtonClick()
    {
        Debug.Log("Game Start 按鈕被點擊！");
        
        if (GameProgressionManager.instance != null) 
        {
            GameProgressionManager.StartGameFlow();
        }
        else
        {
            Debug.LogError("GameProgressionManager 尚未初始化！無法啟動遊戲流程。請檢查 GameProgressionManager 的載入順序或 GameObject 設定。", this);
        }
    }
}