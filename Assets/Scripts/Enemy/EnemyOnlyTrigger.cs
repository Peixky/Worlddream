using UnityEngine;

public class EnemyOnlyTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("🟢 怪物進入觸發區：" + other.name);

            // 你可以在這裡做任何怪物碰到這區域的事情
            // 例如讓怪物停止移動、觸發事件、變得憤怒等等
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("🔵 怪物離開觸發區：" + other.name);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f); // 半透明紅色
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
        }
    }

}
