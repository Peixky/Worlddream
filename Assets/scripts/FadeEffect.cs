using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class FadeEffect : MonoBehaviour 
{
    public Image fadeImage; 
    public float fadeDuration = 1.0f; 
    public float targetAlpha = 0.8f;

    void Awake()
    {
        if (fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
        }
    
        if (fadeImage != null)
        {
            Color currentColor = fadeImage.color;
            currentColor.a = 0f;
            fadeImage.color = currentColor;
        }
    }

    
    public void StartFadeIn()
    {
        if (fadeImage == null) return; 
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一幀
        }
        fadeImage.color = targetColor; // 確保達到目標透明度
    }

    // 如果需要漸變消失 (例如遊戲重啟時)
    public void StartFadeOut()
    {
        if (fadeImage == null) return; // 確保 Image 存在
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // 變回完全透明

        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = targetColor;
    }
}