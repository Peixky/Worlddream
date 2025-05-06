/*
/*using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // 加入這個才能換場景

public class ATM_Story : MonoBehaviour
{
    public static bool isPlayingStory = false;
    public static string nextSceneName = "";

    void Start()
    {
        // 這裡可以做初始化設定
    }

    void Update()
    {
        if (isPlayingStory && Input.anyKeyDown)
        {
            EndStory();
        }
    }

    void StartStory()
    {
        Player_Control.is_lock = true;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        isPlayingStory = true;
    }

    void EndStory()
    {
        isPlayingStory = false;
        Player_Control.is_lock = false;

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void Win()
    {
        StartStory();
        nextSceneName = "NextStage"; // 請把 NextStage 改成你要去的場景名字
    }

    public void Lose()
    {
        StartStory();
        nextSceneName = ""; // 失敗不用換場景
    }

    public void Lose_In_ATM()
    {
        StartStory();
        nextSceneName = ""; // ATM失敗也不用換場景
    }
}
*/
