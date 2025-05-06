using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonSceneLoader : MonoBehaviour
{
    public string sceneName = "Levels/Level1";

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("沒有設定要載入的場景名稱！");
        }
    }
}