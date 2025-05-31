using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("生命設定")]
    [SerializeField] private int maxHealth = 5;
    public int MaxHealth => maxHealth;

    public int CurrentHealth { get; private set; }

    // ✅ 新增：是否死亡的屬性
    public bool IsDead => CurrentHealth <= 0;

    [Header("UI")]
    [SerializeField] private HealthBarUI healthBarUI;

    // ✅ 事件
    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        UpdateHealthUI();
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        Debug.Log($"{gameObject.name} 受到 {amount} 傷害，當前血量：{CurrentHealth}");

        UpdateHealthUI();
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (IsDead)
        {

            OnDied?.Invoke(); // 觸發死亡事件（如 BossHealth 或 PlayerDeathHandler 可接收）
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        Debug.Log($"{gameObject.name} 回復 {amount} 血量，當前血量：{CurrentHealth}");

        UpdateHealthUI();
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    private void UpdateHealthUI()
    {
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(CurrentHealth, MaxHealth);
        }
    }

    // ✅ 提供手動設置血量（可用於存檔或編輯器設置）
    public void SetHealth(int value)
    {
        CurrentHealth = Mathf.Clamp(value, 0, MaxHealth);
        UpdateHealthUI();
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}
