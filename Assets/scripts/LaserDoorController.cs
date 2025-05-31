using UnityEngine;

public class LaserDoorController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D laserCollider;

    [Header("Timing Settings")]
    [SerializeField] private float reopenDelay = 3f;

    private bool isClosed = false;

    public void DeactivateLaser()
    {
        if (isClosed) return;

        isClosed = true;
        animator.SetTrigger("Close");
        laserCollider.enabled = false;

        Invoke(nameof(ReactivateLaser), reopenDelay);
    }

    private void ReactivateLaser()
    {
        isClosed = false;
        animator.SetTrigger("Open");
        laserCollider.enabled = true;
    }
}
