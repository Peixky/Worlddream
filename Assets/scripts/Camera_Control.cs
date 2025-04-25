using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private Transform playerTransform;
    private BoxCollider2D bounds;

    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        cam = Camera.main;
        playerTransform = Game_Manager.instance.player_object.transform;

        GameObject boundObj = GameObject.Find("Camera_Bounding_Collider_1");
        if (boundObj != null)
            bounds = boundObj.GetComponent<BoxCollider2D>();

        Vector3 screenBottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, 10));
        halfWidth = Mathf.Abs(screenBottomLeft.x - cam.transform.position.x);
        halfHeight = Mathf.Abs(screenBottomLeft.y - cam.transform.position.y);
    }

    void Update()
    {
        if (Player_Control.is_lock) return;

        Vector3 targetPos = playerTransform.position;

        if (bounds != null)
        {
            Bounds b = bounds.bounds;

            float minX = b.min.x + halfWidth;
            float maxX = b.max.x - halfWidth;
            float minY = b.min.y + halfHeight;
            float maxY = b.max.y - halfHeight;
            //mathf.lerp可以避免平滑相機的移動，相機就不會移動過快
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

            // Debug 視覺化
            Debug.DrawLine(targetPos + new Vector3(-halfWidth, 0), targetPos + new Vector3(halfWidth, 0), Color.red);
            Debug.DrawLine(targetPos + new Vector3(0, -halfHeight), targetPos + new Vector3(0, halfHeight), Color.red);
        }

        Vector3 smoothPos = Vector3.Lerp(cam.transform.position, new Vector3(targetPos.x, targetPos.y, cam.transform.position.z), 3f * Time.deltaTime);
        cam.transform.position = smoothPos;
    }
}
