using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2 : MonoBehaviour
{
    public void level_option2()
    {
        // 訂閱場景載入完成事件
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 載入 Level2 場景
        SceneManager.LoadSceneAsync("level2");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "level2")
        {
            // 尋找場景中的 ExitTrigger 並關閉
            GameObject exit = GameObject.Find("Storyexit");
            if (exit != null)
            {
                exit.SetActive(false);
                Debug.Log("Level2: Storyexit 已關閉！");
            }
            else
            {
                Debug.LogWarning("Level2: 找不到 Storyexit，請確認名稱正確。");
            }

            GameObject exit2 = GameObject.Find("Levelexit");
            exit2.SetActive(true);

            // 取消事件註冊，避免之後場景切換時多次觸發
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
