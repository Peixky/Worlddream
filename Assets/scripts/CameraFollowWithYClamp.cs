using UnityEngine;

public class CameraFollowWithYClamp : MonoBehaviour
{
    public Transform target;         
    public float smoothSpeed = 5f;  
    public float minY = 0f;   
    public float maxY = 5f;
    public float offsetY = -1.5f; 
    public float offsetZ = -10f; 

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position;

        float offsetYApplied = targetPos.y + offsetY;

        // 限制 Y 軸在範圍內
        float clampedY = Mathf.Clamp(offsetYApplied, minY, maxY);

        // 最終目標位置（X 跟著、Y 限制後 + 偏移、Z 固定）
        Vector3 desiredPos = new Vector3(targetPos.x, clampedY, offsetZ);

        // 平滑移動
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
    }
}
