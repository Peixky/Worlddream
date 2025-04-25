using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterball : MonoBehaviour
{
    public enum WaterballType { Small, Big } // 兩種水球類型
    public WaterballType waterballType = WaterballType.Small;

    public float timer = 1.5f;
    public float speed;
    public int damage;

    private bool isRight = true;
    private SpriteRenderer spr;

    void Start()
    {
        spr = this.GetComponent<SpriteRenderer>();

        // 根據玩家朝向來決定水球方向
        isRight = Player_Control.faced_right;
        if (!isRight)
        {
            spr.flipX = true;
        }

        // 根據水球類型來決定速度 & 傷害
        switch (waterballType)
        {
            case WaterballType.Small:
                speed = 10f;
                damage = 1;
                break;
            case WaterballType.Big:
                speed = 6f;
                damage = 3;
                break;
        }
    }

    void Update()
    {
        // 移動
        Vector3 movement = new Vector3(speed * Time.deltaTime, 0, 0);
        transform.position += isRight ? movement : -movement;

        // 計時刪除
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 假設你有敵人或Boss可以判定
        if (collision.CompareTag("Enemy"))
        {
            // 碰到敵人時造成 damage 傷害（這邊要敵人有個 TakeDamage 函數）
            collision.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
