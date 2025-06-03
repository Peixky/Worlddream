using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public float spawnRange = 10f;

    private Transform player;
    private bool hasSpawned = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (hasSpawned || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= spawnRange)
        {
            Instantiate(monsterPrefab, transform.position, Quaternion.identity);
            hasSpawned = true;
        }
    }
}
