using UnityEngine;

public class TrapActivator : MonoBehaviour
{
    public GameObject hiddenEnemy;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hiddenEnemy.SetActive(true);
            Destroy(gameObject); // 觸發一次後就移除
        }
    }
}
