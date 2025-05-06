using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    public void UpdateCoinText(int amount)
    {
        coinText.text = "x " + amount;
    }
}
