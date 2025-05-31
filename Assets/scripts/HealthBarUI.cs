using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public RectTransform barTransform;  // 血條的 RectTransform
    public float maxWidth = 100f;       // 血條滿血時寬度（與 UI 寬度一致）

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float percent = currentHealth / maxHealth;
        float newWidth = maxWidth * percent;

        Vector2 size = barTransform.sizeDelta;
        size.x = newWidth;
        barTransform.sizeDelta = size;
    }
}
