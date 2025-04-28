using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("边界 (可选)")]
    [Tooltip("限制相机可视范围的 BoxCollider2D")]
    [SerializeField] private BoxCollider2D bounds;

    [Header("平滑参数")]
    [SerializeField, Tooltip("跟随平滑速度，推荐 3~6")] private float smoothSpeed = 3f;

    private Camera cam;
    private Transform playerTransform;
    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        cam = GetComponent<Camera>();

        // 自动查找名为 "Player1" 的角色
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("未找到名为 'Player1' 的游戏对象！");
        }

        // 计算正交相机的半宽半高
        if (cam.orthographic)
        {
            halfHeight = cam.orthographicSize;
            halfWidth  = cam.aspect * halfHeight;
        }
        else
        {
            Debug.LogWarning("当前不是正交相机，无法自动计算 halfWidth/halfHeight，请手动设置。");
        }
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        Vector3 desiredPos = playerTransform.position;

        // 如果设置了边界，并且是正交相机，对目标位置进行限制
        if (bounds != null && cam.orthographic)
        {
            Bounds b = bounds.bounds;
            float minX = b.min.x + halfWidth;
            float maxX = b.max.x - halfWidth;
            float minY = b.min.y + halfHeight;
            float maxY = b.max.y - halfHeight;

            desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
            desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);

            // 可视化边界 (调试用红线)
            Debug.DrawLine(
                desiredPos + new Vector3(-halfWidth, 0f, 0f),
                desiredPos + new Vector3( halfWidth, 0f, 0f), Color.red);
            Debug.DrawLine(
                desiredPos + new Vector3(0f, -halfHeight, 0f),
                desiredPos + new Vector3(0f,  halfHeight, 0f), Color.red);
        }

        // 保持原 Z 值，执行平滑跟随
        Vector3 currentPos = transform.position;
        Vector3 targetPos  = new Vector3(desiredPos.x, desiredPos.y, currentPos.z);
        transform.position = Vector3.Lerp(currentPos, targetPos, smoothSpeed * Time.deltaTime);
    }
}
