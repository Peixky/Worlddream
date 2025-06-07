using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1tohome : MonoBehaviour
{
    [Header("觸發設定")]
    public string playerTag = "Player";         // 玩家 Tag
    public float transitionDelay = 1.0f;         // 延遲幾秒後切場景
    [Header("場景設定")]
    public string menuSceneName = "test-Menu"; // 要載入的場景名稱

    private bool hasTriggered = false;           // 防止重複觸發

    // 3D 觸發
    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            hasTriggered = true;
            Debug.Log($"Level1ExitPoint: 玩家觸發，{transitionDelay} 秒後回到 {menuSceneName}");
            Invoke(nameof(ReturnToMenu), transitionDelay);
        }
    }

    // 2D 觸發
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            hasTriggered = true;
            Debug.Log($"[2D] Level1ExitPoint: 玩家觸發，{transitionDelay} 秒後回到 {menuSceneName}");
            Invoke(nameof(ReturnToMenu), transitionDelay);
        }
    }

    // 載入 test-Menu
    private void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
