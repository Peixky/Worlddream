using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;

public class VideoCutsceneManager : MonoBehaviour
{
    [Header("Video Setup")]
    public VideoPlayer videoPlayer;
    public RawImage videoDisplayImage;

    [Header("Audio Setup")]
    [Tooltip("用來在開始/結束時暫時改變音樂音量")]
    public AudioMixer audioMixer;
    [Tooltip("AudioMixer 裡控制音樂的參數名稱")]
    public string volumeParamName = "music";
    private float originalVolume;
    private bool hasStoredVolume = false;

    private Action onVideoFinishedCallback;
    public static VideoCutsceneManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoCutsceneManager: VideoPlayer 未設定！", this);
            enabled = false;
            return;
        }
        if (videoDisplayImage != null)
            videoDisplayImage.gameObject.SetActive(false);

        // 影片結束回調
        videoPlayer.loopPointReached += HandleVideoFinished;
        // 場景卸載事件，用於檢測退出 Scene3
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= HandleVideoFinished;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void PlayVideo(Action onFinished)
    {
        onVideoFinishedCallback = onFinished;

        if (videoDisplayImage != null)
            videoDisplayImage.gameObject.SetActive(true);

        // 儲存並靜音
        if (audioMixer != null)
        {
            if (!hasStoredVolume)
            {
                audioMixer.GetFloat(volumeParamName, out originalVolume);
                hasStoredVolume = true;
            }
            audioMixer.SetFloat(volumeParamName, -80f);
            Debug.Log("VideoCutsceneManager: 背景音樂已靜音。");
        }
        else Debug.LogWarning("VideoCutsceneManager: AudioMixer 未設定，無法靜音音樂。", this);

        videoPlayer.Play();
        Time.timeScale = 0f;
        Debug.Log("VideoCutsceneManager: 開始播放影片。");
    }

    void HandleVideoFinished(VideoPlayer vp)
{
    Debug.Log("VideoCutsceneManager: 影片播放結束。");

    if (videoDisplayImage != null)
        videoDisplayImage.gameObject.SetActive(false);

    // 取得目前場景名稱（例如 Scene3）
    string currentScene = SceneManager.GetActiveScene().name;

    // 判斷是否要進入 BossDeathScene（假設你是在回呼裡 LoadScene）
    bool goingToBossDeathScene = onVideoFinishedCallback != null; // 視你的設計而定

    // 若不是要去 BossDeathScene，才恢復音量
    if (!goingToBossDeathScene && audioMixer != null && hasStoredVolume)
    {
        audioMixer.SetFloat(volumeParamName, originalVolume);
        Debug.Log("VideoCutsceneManager: 背景音樂音量已恢復。");
    }

    // 執行影片結束後的行為（如切換場景）
    onVideoFinishedCallback?.Invoke();
    Debug.Log("VideoCutsceneManager: 已執行影片結束回調。");

        // 如果從 Scene3 → BossDeathScene，強制靜音處理
        if (currentScene == "Scene3")
        {
            if (audioMixer != null)
            {
                audioMixer.SetFloat(volumeParamName, -80f);
                Debug.Log("VideoCutsceneManager: 進入 BossDeathScene，背景音樂已關閉。");
            }

            if (AudioManager3.Instance != null)
            {
                AudioManager3.Instance.StopMusic();
            }

            if (VolumeSettings3.Instance != null)
            {
                VolumeSettings3.Instance.ForceSilence(); // 或呼叫 Mute() 依你命名
                Debug.Log("VideoCutsceneManager: VolumeSettings3 音樂已關閉。");
            }
        if (VolumeSettings3.Instance != null)
{
    VolumeSettings3.Instance.ForceSilence();
    Debug.Log("VideoCutsceneManager: VolumeSettings3 音樂已關閉。");
}

    }
}


    // 當卸載場景時呼叫
    private void OnSceneUnloaded(Scene scene)
    {
        // 如果是從 Scene3 卸載，則關閉音樂
        if (scene.name == "Scene3")
        {
            if (audioMixer != null)
            {
                audioMixer.SetFloat(volumeParamName, -80f);
                Debug.Log("VideoCutsceneManager: 已在退出 Scene3 時關閉背景音樂。");
            }
        }
    }
}