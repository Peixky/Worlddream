using UnityEngine;

public class AudioManager3 : MonoBehaviour
{
    public static AudioManager3 Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip background;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    public void PlayMusic()
    {
        if (!musicSource.isPlaying)
            musicSource.Play();
    }
}