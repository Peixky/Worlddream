using UnityEngine;
using UnityEngine.UI; // For RawImage
using UnityEngine.Video; // For VideoPlayer
using System; // For Action
using System.Collections; // For coroutines

public class VideoCutsceneManager : MonoBehaviour
{
    [Header("Video Setup")]
    public VideoPlayer videoPlayer; // 從 Hierarchy 拖曳 VideoPlaybackManager 物件上的 VideoPlayer 組件到這裡
    public RawImage videoDisplayImage; // 從 Hierarchy 拖曳 VideoDisplayImage (Canvas 下的 RawImage) 到這裡

    private Action onVideoFinishedCallback; // 用於儲存影片播放結束後要執行的動作

    // 使用靜態實例實現單例模式，讓其他腳本更容易找到它
    public static VideoCutsceneManager Instance { get; private set; }

    void Awake()
    {
        // 單例模式：確保這個腳本在遊戲中只有一個實例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 如果這個管理器需要在場景之間持續存在，就加上 DontDestroyOnLoad
        // 但通常過場動畫管理器只存在於特定的過場動畫場景，或在 Boss 死亡後創建。
        // 如果它只存在於 Boss 死亡的場景，則不需要 DontDestroyOnLoad。
        // 我假設它在 Boss 死亡的場景中，並在 Boss 死亡時被激活。
    }

    void OnEnable()
    {
        // 確保 videoPlayer 已經被賦值
        if (videoPlayer == null)
        {
            Debug.LogError("VideoCutsceneManager: VideoPlayer is not assigned in the Inspector!", this);
            enabled = false;
            return;
        }

        // 隱藏影片顯示，直到我們需要播放
        if (videoDisplayImage != null)
        {
            videoDisplayImage.gameObject.SetActive(false);
        }

        // 訂閱影片播放結束的事件
        videoPlayer.loopPointReached += OnVideoEnd; 
    }

    void OnDisable()
    {
        // 取消訂閱事件，防止物件被銷毀後仍觸發
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    // 公開方法：開始播放影片
    public void PlayVideo(Action onFinished)
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoCutsceneManager: 無法播放影片，VideoPlayer 未設定。", this);
            onFinished?.Invoke(); // 即使影片無法播放，也嘗試執行回調，避免流程卡住
            return;
        }

        onVideoFinishedCallback = onFinished; // 儲存影片播放結束後要執行的回調

        // 顯示影片的 RawImage
        if (videoDisplayImage != null)
        {
            videoDisplayImage.gameObject.SetActive(true); 
        }
        
        videoPlayer.Play(); // 開始播放影片
        Time.timeScale = 0f; // 暫停遊戲邏輯，讓影片獨佔時間
        Debug.Log("VideoCutsceneManager: 開始播放影片過場動畫。");
    }

    // 影片播放結束時觸發
    void OnVideoEnd(VideoPlayer vp)
    {
        Time.timeScale = 1f; // 恢復遊戲時間流動
        Debug.Log("VideoCutsceneManager: 影片播放結束。");

        // 隱藏影片的 RawImage
        if (videoDisplayImage != null)
        {
            videoDisplayImage.gameObject.SetActive(false); 
        }

        // 執行影片播放結束後的回調動作（例如載入場景）
        onVideoFinishedCallback?.Invoke();
    }
}