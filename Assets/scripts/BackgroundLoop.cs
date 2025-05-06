using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public Transform otherBackground;
    public float backgroundWidth;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // 計算畫面左邊界
        float cameraLeftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        // 如果這塊背景的最右邊小於畫面左邊，就搬到另一塊右邊
        float thisRightEdge = transform.position.x + backgroundWidth / 2;

        if (thisRightEdge < cameraLeftEdge)
        {
            transform.position = new Vector3(
                otherBackground.position.x + backgroundWidth,
                transform.position.y,
                transform.position.z
            );
        }
    }
}
