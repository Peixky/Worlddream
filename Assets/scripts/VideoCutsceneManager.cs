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

    [Header("Audio Setup")] // 新增一個 Header
    public AudioSource backgroundMusicSource; // 從 Hierarchy 拖曳 BGM GameObject 上的 AudioSource 組件到這裡

    private Action onVideoFinishedCallback; // 用於儲存影片播放結束後要執行的動作

    public static VideoCutsceneManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoCutsceneManager: VideoPlayer is not assigned in the Inspector!", this);
            enabled = false;
            return;
        }

        if (videoDisplayImage != null)
        {
            videoDisplayImage.gameObject.SetActive(false);
        }

        // 訂閱影片結束事件
        videoPlayer.loopPointReached += HandleVideoFinished; // 修改這裡，使用新的函式
    }

    void OnDisable()
    {
        if (videoPlayer != null)
        {
            // 取消訂閱影片結束事件
            videoPlayer.loopPointReached -= HandleVideoFinished; // 修改這裡
        }
    }

    public void PlayVideo(Action onFinished)
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoCutsceneManager: 無法播放影片，VideoPlayer 未設定。", this);
            onFinished?.Invoke();
            return;
        }

        onVideoFinishedCallback = onFinished;

        if (videoDisplayImage != null)
        {
            videoDisplayImage.gameObject.SetActive(true);
        }

        // 停止背景音樂
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop(); // 停止背景音樂
            Debug.Log("VideoCutsceneManager: 背景音樂已停止。");
        }
        else
        {
            Debug.LogWarning("VideoCutsceneManager: 背景音樂 AudioSource 未設定！無法停止背景音樂。", this);
        }

        videoPlayer.Play();
        Time.timeScale = 0f; // 影片播放期間暫停時間
        Debug.Log("VideoCutsceneManager: 開始播放影片過場動畫。");
    }

    // 新的影片結束處理函式
    void HandleVideoFinished(VideoPlayer vp)
    {
        // 影片結束時不需要恢復 Time.timeScale，因為您說要直接進劇情，
        // 且劇情可能會有自己的時間控制或場景切換。
        // if (Time.timeScale != 0f) Time.timeScale = 1f; // 這一行現在不需要了

        Debug.Log("VideoCutsceneManager: 影片播放結束。");

        if (videoDisplayImage != null)
        {
            // 您可以選擇是否要隱藏畫面，如果下一段劇情是切換場景或完全覆蓋，這行也可以移除。
            // 但為了避免影片殘影，通常還是會隱藏。
            videoDisplayImage.gameObject.SetActive(false);
        }

        // 影片播放結束後立即觸發回調，進入劇情
        // 假設這個 onVideoFinishedCallback 就是用來進入劇情的。
        onVideoFinishedCallback?.Invoke();
        Debug.Log("VideoCutsceneManager: 已執行影片結束後的回調（即將進入劇情）。");
    }
}