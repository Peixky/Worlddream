using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    

    public Image[] hearts; // 在 Inspector 中設置三顆愛心的 Image
    public Sprite fullHeart;  // 正常愛心（紅色）
    public Sprite emptyHeart; // 空愛心（灰色）

    public void UpdateHearts(int currentHP, int maxHP)
{
    Debug.Log("更新愛心！當前血量：" + currentHP + "，最大血量：" + maxHP);  // 檢查當前血量與最大血量

    for (int i = 0; i < hearts.Length; i++)
    {
        if (i < currentHP)
        {
            Debug.Log("設置愛心 " + i + " 為紅心");  // 檢查愛心是否設為紅心
            hearts[i].sprite = fullHeart;
        }
        else
        {
            Debug.Log("設置愛心 " + i + " 為空心");  // 檢查愛心是否設為空心
            hearts[i].sprite = emptyHeart;
        }
    }
}


}
