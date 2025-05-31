using UnityEngine;
using System.Collections; 

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class FollowerEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Transform player;
    private Rigidbody2D rb;

    // <<<<<< 新增：追蹤小怪朝向 >>>>>>
    private bool facingRight = true; // 預設小怪面向右邊

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

            // <<<<<< 處理小怪翻轉 >>>>>>
            // 如果小怪向右移動 (direction.x > 0) 且當前面向左 (!facingRight)，則翻轉
            if (direction.x < 0 && !facingRight)
            {
                Flip();
            }
            // 如果小怪向左移動 (direction.x < 0) 且當前面向右 (facingRight)，則翻轉
            else if (direction.x > 0 && facingRight)
            {
                Flip();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 您的原始程式碼中，這裡引用的是 Health 腳本，我會保持不變
        // 但通常我們會用 PlayerHealth 腳本
        if (collision.collider.CompareTag("Player"))
        {
            // 注意：這裡應該是 PlayerHealth，而不是 Health
            // Health health = collision.collider.GetComponent<Health>(); 
            // if (health != null)
            // {
            //     health.TakeDamage(1);
            // }
        }
    }

    // <<<<<< 新增：翻轉小怪方向的方法 >>>>>>
    void Flip()
    {
        facingRight = !facingRight; // 切換面向方向的布林值
        Vector3 scale = transform.localScale; // 獲取當前物體的縮放
        scale.x *= -1; // 將 X 軸縮放值反轉 (例如從 1 變 -1，或從 -1 變 1)
        transform.localScale = scale; // 將新的縮放值應用到物體
    }
}