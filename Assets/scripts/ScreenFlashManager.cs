using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScreenFlashManager : MonoBehaviour
{
    [Header("Screen Flash Setup")]
    public Image flashPanel; 
    public float flashDuration = 0.5f; // 單次閃爍的總持續時間 (淡入+淡出)
    public float maxAlpha = 0.8f; // 閃爍時的最大透明度

    public static ScreenFlashManager Instance { get; private set; }

    private bool isFlashing = false; // <<<< 新增：控制是否持續閃爍的旗標 >>>>

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (flashPanel == null)
        {
            flashPanel = GetComponent<Image>();
        }

        if (flashPanel == null)
        {
            Debug.LogError("ScreenFlashManager: Flash Panel Image 組件未設定或未找到！請確保腳本掛載在帶 Image 的物件上。", this);
            enabled = false;
            return;
        }

        Color c = flashPanel.color;
        c.a = 0f;
        flashPanel.color = c;
        flashPanel.gameObject.SetActive(false); 
    }

    // 公開方法：觸發畫面開始持續閃爍
    public void FlashScreen()
    {
        if (flashPanel == null || !enabled) return;

        isFlashing = true; // <<<< 設定為開始持續閃爍 >>>>
        flashPanel.gameObject.SetActive(true); 

        StopAllCoroutines(); 
        StartCoroutine(FlashRoutine()); 
    }

    // <<<< 新增：公開方法，用於停止畫面閃爍 >>>>
    public void StopFlashing()
    {
        isFlashing = false; // 設定旗標為停止
        StopAllCoroutines(); // 停止所有協程 (包括 FlashRoutine)
        if (flashPanel != null)
        {
            Color c = flashPanel.color;
            c.a = 0f; // 將 Panel 設為完全透明
            flashPanel.color = c;
            flashPanel.gameObject.SetActive(false); // 隱藏 Panel
        }
        Debug.Log("ScreenFlashManager: 畫面閃紅已停止。");
    }

    IEnumerator FlashRoutine()
    {
        while (isFlashing) // <<<< 迴圈：只要 isFlashing 為 true 就一直閃 >>>>
        {
            // 階段 1: 淡入 (Fade In)
            float timer = 0f;
            Color startColor = flashPanel.color;
            startColor.a = 0f; // 每次淡入都從完全透明開始
            flashPanel.color = startColor;

            Color targetColor = startColor;
            targetColor.a = maxAlpha;

            while (timer < flashDuration / 2f)
            {
                timer += Time.deltaTime;
                flashPanel.color = Color.Lerp(startColor, targetColor, timer / (flashDuration / 2f));
                yield return null;
            }
            flashPanel.color = targetColor;

            // 階段 2: 淡出 (Fade Out)
            timer = 0f;
            startColor = flashPanel.color; 
            targetColor = startColor;
            targetColor.a = 0f;

            while (timer < flashDuration / 2f)
            {
                timer += Time.deltaTime;
                flashPanel.color = Color.Lerp(startColor, targetColor, timer / (flashDuration / 2f));
                yield return null;
            }
            flashPanel.color = targetColor;

            // 如果迴圈繼續，會在下一次迭代時再次從透明開始淡入
            // 可以在這裡加一個短暫的停頓，讓閃爍效果更明顯
            // yield return new WaitForSecondsRealtime(0.1f); 
        }
        // 當 isFlashing 變為 false 時，迴圈結束，協程也會結束
        flashPanel.gameObject.SetActive(false); // 確保在迴圈結束後隱藏 Panel
    }
}