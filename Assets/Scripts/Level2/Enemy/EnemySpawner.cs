using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("生成設定")]
    public GameObject enemyPrefab;
    public float spawnRadius = 15f;
    public Vector3 spawnOffset = Vector3.zero;
    public bool destroyWhenOutOfRange = true;

    private GameObject spawnedEnemy;
    private Transform player;
    private bool hasSpawnedAndDied = false; // ✅ 新增：敵人是否已死亡過

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || enemyPrefab == null || hasSpawnedAndDied) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= spawnRadius)
        {
            if (spawnedEnemy == null)
            {
                Vector3 spawnPosition = transform.position + spawnOffset;
                spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // ✅ 綁定死亡事件：讓敵人死後告訴我們
                var enemyHealth = spawnedEnemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.OnDied += HandleEnemyDeath;
                }
            }
        }
        else
        {
            if (destroyWhenOutOfRange && spawnedEnemy != null)
            {
                Destroy(spawnedEnemy);
                spawnedEnemy = null;
            }
        }
    }

    // ✅ 新增：當敵人死亡，設標記為已死亡
    private void HandleEnemyDeath()
    {
        hasSpawnedAndDied = true;
        spawnedEnemy = null;
        Debug.Log("EnemySpawner: 敵人死亡，不再重生！");

        if (spawnedEnemy != null)
        {
            var health = spawnedEnemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.OnDied -= HandleEnemyDeath;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + spawnOffset, 0.3f);
    }
}
