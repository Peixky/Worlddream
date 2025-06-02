using UnityEngine;
using UnityEngine.UI; // 使用 UnityEngine.UI.Image 需要這個命名空間
using System.Collections; // 使用協程 (IEnumerator, StartCoroutine, WaitForSecondsRealtime) 需要這個命名空間

public class FadeEffect : MonoBehaviour // <<<< 這裡的類名必須是 FadeEffect >>>>>
{
    public Image fadeImage; // 拖曳 FadePanel 自己的 Image 元件到這裡
    public float fadeDuration = 1.0f; // 漸變時間 (秒)
    public float targetAlpha = 0.8f; // 目標透明度 (0.0 完全透明 - 1.0 完全不透明)

    void Awake()
    {
        // 如果 fadeImage 沒有在 Inspector 中設置，嘗試自動獲取
        if (fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
        }
        // 確保初始是完全透明
        if (fadeImage != null)
        {
            Color currentColor = fadeImage.color;
            currentColor.a = 0f;
            fadeImage.color = currentColor;
        }
    }

    // 當需要漸變時呼叫此方法
    public void StartFadeIn()
    {
        if (fadeImage == null) return; // 確保 Image 存在
        StopAllCoroutines(); // 停止任何正在運行的漸變協程
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