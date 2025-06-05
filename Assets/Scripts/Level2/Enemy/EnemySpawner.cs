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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || enemyPrefab == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= spawnRadius)
        {
            if (spawnedEnemy == null)
            {
                Vector3 spawnPosition = transform.position + spawnOffset;
                spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + spawnOffset, 0.3f);
    }
}
