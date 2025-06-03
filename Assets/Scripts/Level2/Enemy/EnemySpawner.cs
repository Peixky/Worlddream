using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRadius = 15f;

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

        if (distance <= spawnRadius && spawnedEnemy == null)
        {
            spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
        else if (distance > spawnRadius && spawnedEnemy != null)
        {
            Destroy(spawnedEnemy);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
