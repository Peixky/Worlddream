using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1 : MonoBehaviour
{
    public void level_option1()
    {
        // 加入場景載入後的事件處理
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 載入場景
        SceneManager.LoadSceneAsync("Scene1");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 確保是目標場景
        if (scene.name == "Scene1")
        {
            // 嘗試尋找 ExitTrigger 物件
            GameObject exit = GameObject.Find("ExitTrigger");
            if (exit != null)
            {
                exit.SetActive(false);
                Debug.Log("ExitTrigger 已被關閉！");
            }
            else
            {
                Debug.LogWarning("找不到 ExitTrigger，請確認物件名稱是否正確。");
            }

            // ✅ 一定要取消訂閱，避免未來場景切換重複執行
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
