using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3; // 最大血量
    private int currentHealth;

    [SerializeField] private PlayerHealthUI healthUI; // 引用 PlayerHealthUI

    private void Start()
    {
        currentHealth = maxHealth;
        healthUI.UpdateHearts(currentHealth, maxHealth); // 初始化顯示愛心
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " 扣血！剩餘血量：" + currentHealth);  // 檢查血量減少

        // 更新顯示
        healthUI.UpdateHearts(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        Debug.Log(gameObject.name + " 死亡");
        gameObject.SetActive(false); // 暫時讓角色消失
    }
}
