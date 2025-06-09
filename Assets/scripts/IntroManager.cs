using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class IntroManager : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject introPanel;
    public GameObject startTextPanel;
    public GameObject fadePanel;
    public TextMeshProUGUI loserTextUI;

    [Header("時間設定")]
    public float vsScreenDuration = 3f;
    public float startTextDuration = 3f;
    public float gameOverFadeDelay = 1f;

    [Header("音效設定")]
    public AudioClip gameStartSoundEffect;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Time.timeScale = 0f;
        GameProgressionManager.currentGameState = GameProgressionManager.GameState.Intro;
        SetUIActive(false);
        StartCoroutine(IntroRoutine());
    }

    void OnEnable()
    {
        GameEvents.OnPlayerDied += ShowGameOver;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerDied -= ShowGameOver;
    }

    IEnumerator IntroRoutine()
    {
        introPanel?.SetActive(true);
        yield return new WaitForSecondsRealtime(vsScreenDuration);

        introPanel?.SetActive(false);
        startTextPanel?.SetActive(true);
        var startText = startTextPanel.GetComponentInChildren<TextMeshProUGUI>(true);
        if (startText != null)
        {
            startText.text = "GAME START!";
            startText.gameObject.SetActive(true);
        }

        if (gameStartSoundEffect != null)
            audioSource.PlayOneShot(gameStartSoundEffect);

        yield return new WaitForSecondsRealtime(startTextDuration);
        startTextPanel?.SetActive(false);

        Time.timeScale = 1f;
        GameProgressionManager.currentGameState = GameProgressionManager.GameState.Playing;
        GameEvents.OnGameStart?.Invoke();
    }

    public static void ShowGameOver()
    {
        var instance = FindFirstObjectByType<IntroManager>();
        if (instance == null) return;

        Time.timeScale = 0f;
        GameProgressionManager.currentGameState = GameProgressionManager.GameState.GameOver;
        instance.fadePanel?.SetActive(true);
        instance.StartCoroutine(instance.ShowLoserTextFlow());
    }

    IEnumerator ShowLoserTextFlow()
    {
        yield return new WaitForSecondsRealtime(gameOverFadeDelay);

        if (loserTextUI != null)
        {
            loserTextUI.gameObject.SetActive(true);
            loserTextUI.text = "GAME OVER";
        }

        yield return new WaitForSecondsRealtime(2f);
        GameProgressionManager.AdvanceStory();
        GameProgressionManager.LoadNextStoryScene();
    }

    void SetUIActive(bool active)
    {
        introPanel?.SetActive(active);
        startTextPanel?.SetActive(active);
        fadePanel?.SetActive(active);
        if (loserTextUI != null) loserTextUI.gameObject.SetActive(active);
    }
}