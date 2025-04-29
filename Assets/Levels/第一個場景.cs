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

    public void Lose_In_第一個場景故事()
    {
        StartStory("Lose_In_第一個場景故事");
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

/*void Start()
{
    flowerSystem = FlowerManager.Instance.GetFlowerSystem(defaultSceneName);
    StartStory("第一個場景故事");
}
修改故事名稱StartStory("第一個場景故事"); 中的字串名稱即可，對應到你 Resources 裡的對話檔名（.txt）

只要按任何按鍵就可以繼續劇情

對話中結束後的事情

public void Win()
{
    StartStory("Win", GoToNextStage);
}
當 "Win" 的故事對話播放完，會自動呼叫 GoToNextStage()

void GoToNextStage()
{
    可以加入我要的ˋ事件
    SceneManager.LoadScene("下一關場景名稱");
}

改鏡頭開場位置
Camera.main.transform.position = new Vector3(0, 0, -10);
這行能夠改變鏡頭切入的位置

Player_Control.is_lock = true;
這行會導致故事開啟，角色被鎖住不會移動
*/
