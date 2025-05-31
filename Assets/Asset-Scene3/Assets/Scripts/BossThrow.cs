using UnityEngine;

public class BossThrow : MonoBehaviour
{
    public GameObject[] projectiles; // 拖入兩種物品 prefab
    public Transform throwPoint;     // 丟出位置（可設在 Boss 手上）
    public float throwForce = 10f;
    public float minInterval = 2f;
    public float maxInterval = 5f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ThrowRoutine());
    }

    System.Collections.IEnumerator ThrowRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);
            ThrowAtPlayer();
        }
    }

    void ThrowAtPlayer()
    {
        if (projectiles.Length == 0 || player == null) return;

        // 隨機選一個物件
        GameObject selected = projectiles[Random.Range(0, projectiles.Length)];

        // 生成
        GameObject proj = Instantiate(selected, throwPoint.position, Quaternion.identity);

        // 計算方向
        Vector2 dir = (player.position - throwPoint.position).normalized;

        // 加力道
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * throwForce;
        }
    }
}
