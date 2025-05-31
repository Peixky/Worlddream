using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 1;

    [Tooltip("1 = 子彈往右, -1 = 子彈往左，用來決定玩家被打時的反彈方向")]
    public int direction = 1;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
