using System.Collections;
using UnityEngine;
using Flower;

public class ATM_Story : MonoBehaviour
{
    FlowerSystem flowerSystem;
    const string defaultSceneName = "default";
    bool isStoryPlaying = false;
    System.Action onStoryEndCallback;

    void Start()
    {
        flowerSystem = FlowerManager.Instance.GetFlowerSystem(defaultSceneName);
        StartStory("第一個場景故事");
    }

    void Update()
    {
        if (!isStoryPlaying)
            return;

        if (Input.anyKeyDown)
        {
            bool isFinished = flowerSystem.Next();
            if (isFinished)
            {
                EndStory();
            }
        }
    }

    void StartStory(string storyName, System.Action onEnd = null)
    {
        Player_Control.is_lock = true;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        isStoryPlaying = true;
        onStoryEndCallback = onEnd;
        flowerSystem.ReadTextFromResource(storyName);
    }

    void EndStory()
    {
        isStoryPlaying = false;
        Player_Control.is_lock = false;
        onStoryEndCallback?.Invoke();
    }

    public void Lose_In_ATM()
    {
        StartStory("Lose_In_ATM");
    }

    public void Win()
    {
        StartStory("Win", GoToNextStage);
    }

    public void Lose()
    {
        StartStory("Lose");
    }

    void GoToNextStage()
    {
        Debug.Log("故事結束了，載入下一個場景或觸發下一個事件！");
        // 這裡可以加上換場景的程式，比如 SceneManager.LoadScene("下一個場景名字");
    }
}
