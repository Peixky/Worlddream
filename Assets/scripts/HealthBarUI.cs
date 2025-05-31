using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("血條設定")]
    public Image fillImage;              // 拖入要縮放的血條 Image（type: Filled）
    public bool fillFromRight = false;   // 設 true 表示從右邊開始縮減

    private void Awake()
    {
        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = fillFromRight ? 1 : 0; // 1=Right, 0=Left
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillImage == null) return;

        float percent = Mathf.Clamp01(currentHealth / maxHealth);
        fillImage.fillAmount = percent;
    }
}
