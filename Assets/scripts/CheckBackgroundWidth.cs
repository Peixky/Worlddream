using UnityEngine;

public class CheckBackgroundWidth : MonoBehaviour
{
    void Start()
    {
        float width = GetComponent<SpriteRenderer>().bounds.size.x;
        Debug.Log("背景的世界寬度是：" + width);
    }
}
