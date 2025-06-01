using UnityEngine;

public class KnockbackProjectile : MonoBehaviour
{
    public float pushForce = 90f;
    public float maxPushTime = 2f; // 持續推力時間（可選）

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDamageHandler damageHandler = other.GetComponent<PlayerDamageHandler>();
            if (damageHandler != null)
            {
                float direction = transform.position.x > other.transform.position.x ? -1f : 1f;

                if (damageHandler.IsInvincible())
                {
                    // ✅ 無敵時只擊退
                    damageHandler.ApplyKnockbackOnly(direction);
                }
                else
                {
                    // ✅ 正常狀態：扣血 + 擊退
                    damageHandler.TakeHitFromShockwave(1, direction);
                }
            }

            Destroy(gameObject);
        }
    }



    // 防止物件永遠存在（可選）
    private void Start()
    {
        Destroy(gameObject, maxPushTime); // 自動移除（例如2秒）
    }
}
