using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DeathUIController : MonoBehaviour
{
    public Image fadePanel;
    public TextMeshProUGUI loserText;

    public float fadeDuration = 2f;
    public float maxFadeAlpha = 100000.0f; //控制暗度

    private void Awake()
    {
        // 初始化：畫面不透明度為 0，LOSER 字也隱藏
        SetAlpha(fadePanel, 0);
        SetAlpha(loserText, 0);
    }

    public void PlayDeathUI()
    {
        StartCoroutine(FadeAndShowText());
    }

    IEnumerator FadeAndShowText()
    {
        // 漸暗背景
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, maxFadeAlpha, elapsed / fadeDuration);
            SetAlpha(fadePanel, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetAlpha(fadePanel, maxFadeAlpha);

        // 顯示 LOSER
        SetAlpha(loserText, 1f);
    }

    void SetAlpha(Graphic g, float a)
    {
        Color c = g.color;
        c.a = a;
        g.color = c;
    }
}
