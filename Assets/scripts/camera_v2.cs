using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("玩家名稱")]
    [Tooltip("要跟隨的玩家 GameObject 名稱")]
    [SerializeField] private string playerName = "456";

    [Header("邊界 (可選)")]
    [SerializeField] private BoxCollider2D bounds;

    [Header("平滑速度")]
    [SerializeField] private float smoothSpeed = 3f;

    private Camera cam;
    private Transform playerTransform;
    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        cam = GetComponent<Camera>();

<<<<<<< HEAD:Assets/scripts/Camera_Control.cs
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
=======
>>>>>>> d8ac183583c8e17b59723999eb2ff318d1b69e95:Assets/scripts/camera_v2.cs
        if (cam.orthographic)
        {
            halfHeight = cam.orthographicSize;
            halfWidth = cam.aspect * halfHeight;
        }
        else
        {
            Debug.LogWarning("非正交相機，請確認相機類型！");
        }

        TryFindPlayer();
    }

    void Update()
    {
        // 如果角色不存在，重新搜尋（角色換了或被刪除時）
        if (playerTransform == null)
        {
            TryFindPlayer();
            return; // 等下一幀再移動相機
        }
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        Vector3 desiredPos = playerTransform.position;

        if (bounds != null && cam.orthographic)
        {
            Bounds b = bounds.bounds;
            float minX = b.min.x + halfWidth;
            float maxX = b.max.x - halfWidth;
            float minY = b.min.y + halfHeight;
            float maxY = b.max.y - halfHeight;

            desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
            desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(desiredPos.x, desiredPos.y, currentPos.z);
        transform.position = Vector3.Lerp(currentPos, targetPos, smoothSpeed * Time.deltaTime);
    }

    private void TryFindPlayer()
    {
        GameObject playerObj = GameObject.Find(playerName);
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            Debug.Log($"已找到角色：{playerName}");
        }
        else
        {
            Debug.LogWarning($"找不到名為「{playerName}」的物件！");
        }
    }
}
