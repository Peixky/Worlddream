using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject settingsPanel; 

    public void StartGame()
    {
        SceneManager.LoadScene("Scene1"); 
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true); 
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); 
    }
}
