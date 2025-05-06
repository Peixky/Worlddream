using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public Transform player;          // 玩家位置
    public Transform otherBackground; // 另一張背景
    public float backgroundWidth;     // 背景圖的寬度（單位距離）

    void Update()
    {
        // 判斷玩家是否已經超過這張背景的寬度
        // 使用 Player 的 X 軸位置減去背景的 X 軸位置來判斷背景是否超過，並且加上一個預設的緩衝區域 (例如 0.1f)，避免太早觸發
        if (player.position.x - transform.position.x >= backgroundWidth * 0.9f)
        {
            // 把這張背景移動到另一張背景的右邊，保持背景寬度的距離
            transform.position = new Vector3(otherBackground.position.x + backgroundWidth, transform.position.y, transform.position.z);
        }
    }
}
