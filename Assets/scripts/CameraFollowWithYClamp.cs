using UnityEngine;

public class CameraFollowWithYClamp : MonoBehaviour
{
    public Transform target;         // 玩家
    public float smoothSpeed = 5f;   // 平滑速度
    public float minY = 0f;          // Y 最低高度
    public float maxY = 5f;          // Y 最高高度
    public float offsetY = -1.5f;    // Y 軸偏移（往下看）
    public float offsetZ = -10f;     // Z 軸固定

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position;

        // 加上 Y 軸偏移
        float offsetYApplied = targetPos.y + offsetY;

        // 限制 Y 軸在範圍內
        float clampedY = Mathf.Clamp(offsetYApplied, minY, maxY);

        // 最終目標位置（X 跟著、Y 限制後 + 偏移、Z 固定）
        Vector3 desiredPos = new Vector3(targetPos.x, clampedY, offsetZ);

        // 平滑移動
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
    }
}
