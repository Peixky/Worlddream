using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("連結平台")]
    public MovingPlatformController targetPlatform; 

    [Header("觸發設定")]
    public string playerTag = "Player"; 
    public bool activateOnce = true; 
    private bool hasActivated = false; 

    [Header("視覺回饋")]
    public SpriteRenderer spriteRenderer; 
    public Sprite buttonUpSprite; 
    public Sprite buttonDownSprite; 

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (spriteRenderer != null && buttonUpSprite != null)
        {
            spriteRenderer.sprite = buttonUpSprite;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && targetPlatform != null && (!activateOnce || !hasActivated))
        {
            if (spriteRenderer != null && buttonDownSprite != null)
            {
                spriteRenderer.sprite = buttonDownSprite;
            }

            // 通知平台控制器開始移動
            targetPlatform.StartPlatformMoveFromZero(); 
            
            hasActivated = true;
        }
    }
    public void ResetButton() 
    {
        if (spriteRenderer != null && buttonUpSprite != null)
        {
            spriteRenderer.sprite = buttonUpSprite;
        }
        hasActivated = false; 
    }
}