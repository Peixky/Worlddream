using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("連結平台")]
    public MovingPlatformController targetPlatform; // 將 MovingPlatformController 腳本拖曳到這裡

    [Header("觸發設定")]
    public string playerTag = "Player"; // 觸發者的 Tag (確保 Player 物件的 Tag 是 "Player")
    public bool activateOnce = true; // 是否只激活一次（建議為 false，讓按鈕可重複使用）
    private bool hasActivated = false; // 是否已經被激活過

    [Header("視覺回饋")]
    public SpriteRenderer spriteRenderer; // 拖曳按鈕自己的 SpriteRenderer 到這裡
    public Sprite buttonUpSprite; // 玩家未踩下時的圖片
    public Sprite buttonDownSprite; // 玩家踩下後或激活時的圖片

    void Start()
    {
        // 如果 spriteRenderer 沒有在 Inspector 中設置，嘗試自動獲取
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        // 確保遊戲開始時顯示未踩下的圖片
        if (spriteRenderer != null && buttonUpSprite != null)
        {
            spriteRenderer.sprite = buttonUpSprite;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 只有當觸發者是 Player 且尚未激活時才執行
        // activateOnce=true: 只觸發一次
        // activateOnce=false: 每次踩下都觸發 (只要平台不在移動中)
        if (other.CompareTag(playerTag) && targetPlatform != null && (!activateOnce || !hasActivated))
        {
            //Debug.Log(gameObject.name + " 按鈕被玩家踩下！", this);
            
            // 改變按鈕圖片為踩下狀態
            if (spriteRenderer != null && buttonDownSprite != null)
            {
                spriteRenderer.sprite = buttonDownSprite;
            }

            // 通知平台控制器開始移動
            targetPlatform.StartPlatformMoveFromZero(); 
            
            hasActivated = true; // 標記為已激活
        }
    }

    // 這個方法由 MovingPlatformController 在平台返回後呼叫，用於重置按鈕圖片
    public void ResetButton() 
    {
        // 只有當按鈕不是單次激活模式，或者當你需要強制重置時才恢復圖片
        // 這裡確保即使是單次激活的按鈕，也能被外部重置圖片
        if (spriteRenderer != null && buttonUpSprite != null)
        {
            spriteRenderer.sprite = buttonUpSprite;
        }
        hasActivated = false; // 重置激活狀態，以便下次可以再次激活
        //Debug.Log(gameObject.name + " 按鈕已重置。");
    }

    // 可選：當玩家離開按鈕時，圖片回到未踩下狀態 (如果您希望按鈕可以重複使用且即時恢復)
    // 但因為平台會自動復位並呼叫 ResetButton，所以這段在您目前的邏輯下可以選擇不用
    // void OnTriggerExit2D(Collider2sD other)
    // {
    //     if (other.CompareTag(playerTag) && !activateOnce) // 只有非單次激活才在離開時處理
    //     {
    //         // 如果平台還在移動，可能不希望立刻恢復按鈕狀態
    //         // 這裡的邏輯需要您自行決定是否要即時恢復，還是等平台復位再恢復
    //         // 在您目前的邏輯中，平台回歸後會呼叫 ResetButton，所以這段可以不加
    //     }
    // }
}