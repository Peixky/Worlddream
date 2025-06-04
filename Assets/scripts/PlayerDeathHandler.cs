using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerDeathHandler : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDied += HandlePlayerDeath; // 將方法名改為更明確的 HandlePlayerDeath
    }

    private void OnDisable()
    {
        health.OnDied -= HandlePlayerDeath; // 將方法名改為更明確的 HandlePlayerDeath
    }

    // 當玩家死亡時呼叫此方法
    private void HandlePlayerDeath() // 修改方法名以反映其新職責
    {
        // 不再呼叫 RespawnManager.Instance.Respawn(gameObject);
        
        // 呼叫 IntroManager 來顯示遊戲結束畫面 (包含 Fade Panel 和 Loser Text)
        IntroManager.ShowGameOver(); 
        
        Debug.Log("玩家死亡，顯示遊戲結束畫面。"); // 更新 Log 訊息
    }
}