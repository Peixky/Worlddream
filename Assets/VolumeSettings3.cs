using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings3 : MonoBehaviour
{
    public static VolumeSettings3 Instance { get; private set; }
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    
    public void ForceSilence()
{
    myMixer.SetFloat("music", -80f);
}

    

private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject);
}

    private void Start()
    {
        // 初始化滑桿位置，如果你想要從 Mixer 讀取初始值：
        float currentVol;
        myMixer.GetFloat("music", out currentVol);
        musicSlider.value = Mathf.Pow(10, currentVol / 20f);

        // 綁定滑動事件
        musicSlider.onValueChanged.AddListener(_ => SetMusicVolume());
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
    }

    /// <summary>暫存當前滑桿，並靜音</summary>
    public void MuteMusic()
    {
        SetMusicVolume();            // 儲存最新值
        myMixer.SetFloat("music", -80f);
    }

    /// <summary>恢復滑桿對應的音量</summary>
    public void UnmuteMusic()
    {
        SetMusicVolume();
    }
}