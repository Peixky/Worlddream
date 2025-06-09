using UnityEngine;
using UnityEngine.UI; // For RawImage
using UnityEngine.Video; // For VideoPlayer
using System; // For Action
using System.Collections; // For coroutines

public class VideoCutsceneManager : MonoBehaviour
{

    [Header("Video Setup")]
    public VideoPlayer videoPlayer; 
    public RawImage videoDisplayImage;

    [Header("Audio Setup")] 
    public AudioSource backgroundMusicSource; 

    private Action onVideoFinishedCallback; 

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

        videoPlayer.loopPointReached += HandleVideoFinished; 
    }

    void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= HandleVideoFinished; 
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
            backgroundMusicSource.Stop(); 
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

    
    void HandleVideoFinished(VideoPlayer vp)
    {
        Debug.Log("VideoCutsceneManager: 影片播放結束。");

        if (videoDisplayImage != null)
        {
            videoDisplayImage.gameObject.SetActive(false);
        }
        onVideoFinishedCallback?.Invoke();
        Debug.Log("VideoCutsceneManager: 已執行影片結束後的回調（即將進入劇情）。");
    }
}