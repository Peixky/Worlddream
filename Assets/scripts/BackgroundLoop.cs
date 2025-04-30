using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public Transform player;          // 玩家位置
    public Transform otherBackground; // 另一張背景
    public float backgroundWidth;     // 背景圖的寬度（單位距離）

    void Update()
    {
        // 檢查玩家是否已經移動到這張背景的右側一段距離
        if (player.position.x - transform.position.x >= backgroundWidth)
        {
            // 把這張背景移動到另一張背景的右邊
            transform.position = new Vector3(otherBackground.position.x + backgroundWidth, transform.position.y, transform.position.z);
        }
    }
}
