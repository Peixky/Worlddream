using UnityEngine;

public class TransparentWall : MonoBehaviour
{
    private SpriteRenderer sr;
    public float transparentAlpha = 0.4f;
    public float fadeSpeed = 5f;

    private float targetAlpha = 1f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Color c = sr.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        sr.color = c;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetAlpha = transparentAlpha; // 變透明
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetAlpha = 1f; // 回復不透明
        }
    }
}
