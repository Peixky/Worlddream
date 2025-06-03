using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    public Transform player;
    public Transform[] backgrounds; // 要填入 A 和 B
    private float backgroundWidth;

    private Transform leftBg;
    private Transform rightBg;

    void Start()
    {
        if (backgrounds.Length != 2)
        {
            Debug.LogError("需要 2 張背景圖！");
            enabled = false;
            return;
        }

        leftBg = backgrounds[0];
        rightBg = backgrounds[1];

        backgroundWidth = leftBg.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        if (player.position.x > rightBg.position.x)
        {
            leftBg.position = rightBg.position + Vector3.right * backgroundWidth;
            Swap();
        }
        else if (player.position.x < leftBg.position.x)
        {
            rightBg.position = leftBg.position + Vector3.left * backgroundWidth;
            Swap();
        }
    }

    void Swap()
    {
        Transform temp = leftBg;
        leftBg = rightBg;
        rightBg = temp;
    }
}
