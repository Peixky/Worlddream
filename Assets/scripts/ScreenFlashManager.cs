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

    private bool isFlashing = false; 

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

   
    public void FlashScreen()
    {
        if (flashPanel == null || !enabled) return;

        isFlashing = true;
        flashPanel.gameObject.SetActive(true); 

        StopAllCoroutines(); 
        StartCoroutine(FlashRoutine()); 
    }

   
    public void StopFlashing()
    {
        isFlashing = false;
        StopAllCoroutines(); 
        if (flashPanel != null)
        {
            Color c = flashPanel.color;
            c.a = 0f; 
            flashPanel.color = c;
            flashPanel.gameObject.SetActive(false); 
        }
        Debug.Log("ScreenFlashManager: 畫面閃紅已停止。");
    }

    IEnumerator FlashRoutine()
    {
        while (isFlashing) 
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

            
        }
       
        flashPanel.gameObject.SetActive(false);
    }
}