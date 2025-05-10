using UnityEngine;

public class CameraFollowWithYClamp : MonoBehaviour
{
    public Transform target;         // 玩家
    public float smoothSpeed = 5f;   // 平滑速度
    public float minY = 0f;          // Y 最低高度
    public float maxY = 5f;          // Y 最高高度
    public float offsetZ = -10f;     // 相機 Z 軸（保持不變）

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position;

        // 限制 Y 軸範圍
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

        // 組合目標位置：X 跟著，Y 限制範圍內，Z 固定
        Vector3 desiredPos = new Vector3(targetPos.x, clampedY, offsetZ);

        // 平滑移動
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
    }
}