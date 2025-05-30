using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject settingsPanel;  // Optional: 設定 UI 面板

    public void StartGame()
    {
        SceneManager.LoadScene("Scene1");  // 請換成你遊戲主場景名稱
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);  // 顯示設定畫面
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);  // 關閉設定畫面
    }

    public void ExitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();  // 這行只會在 build 後有效，編輯器看不到效果
    }
}
