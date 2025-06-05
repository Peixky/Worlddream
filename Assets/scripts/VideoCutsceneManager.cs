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

        videoPlayer.loopPointReached += OnVideoEnd; 
    }

    void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
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
        Time.timeScale = 0f; 
        Debug.Log("VideoCutsceneManager: 開始播放影片過場動畫。");
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Time.timeScale = 1f; 
        Debug.Log("VideoCutsceneManager: 影片播放結束。");

        if (videoDisplayImage != null)
        {
            videoDisplayImage.gameObject.SetActive(false); 
        }

        // 移除：影片結束後不再恢復背景音樂的程式碼

        onVideoFinishedCallback?.Invoke();
    }
}