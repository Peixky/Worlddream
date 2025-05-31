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

    // 從右到左顯示愛心
    for (int i = 0; i < hearts.Length; i++)
    {
        int heartIndex = hearts.Length - 1 - i; // 愛心從右往左變
        if (i < currentHP)
        {
            Debug.Log("設置愛心 " + heartIndex + " 為紅心");  // 確認設置為紅心
            hearts[heartIndex].sprite = fullHeart;
        }
        else
        {
            Debug.Log("設置愛心 " + heartIndex + " 為空心");  // 確認設置為空心
            hearts[heartIndex].sprite = emptyHeart;
        }
    }
}



}
