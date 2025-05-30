using UnityEngine;

public class LaserButton : MonoBehaviour
{
    [SerializeField] private LaserDoorController laserDoor;
    [SerializeField] private Animator animator;
    [SerializeField] private float pressDuration = 1.5f;

    private bool isPressing = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPressing && other.CompareTag("Player"))
        {
            isPressing = true;

            // 播放壓下動畫
            animator.SetTrigger("Pressed");

            // 啟動雷射關閉流程
            laserDoor.DeactivateLaser();

            // 幾秒後恢復按鈕
            Invoke(nameof(ResetButton), pressDuration);
        }
    }

    private void ResetButton()
    {
        // 播放彈回動畫
        animator.SetTrigger("Reset");
        isPressing = false;
    }
}
