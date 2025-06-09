using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("血條設定")]
    public Image fillImage;           
    public bool fillFromRight = false;   

    private void Awake()
    {
        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = fillFromRight ? 1 : 0; 
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillImage == null) return;

        float percent = Mathf.Clamp01(currentHealth / maxHealth);
        fillImage.fillAmount = percent;
    }
}
