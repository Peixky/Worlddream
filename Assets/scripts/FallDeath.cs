using UnityEngine;

public class FallDeath : MonoBehaviour
{
    public float fallDeathY = -10f; // 設定死亡的最低 Y 值
    private Animator anim;
    private PlayerController movementScript;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        movementScript = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isDead) return;

        if (transform.position.y < fallDeathY)
        {
            DieByFalling();
        }
    }

    void DieByFalling()
    {
        isDead = true;
        anim.SetBool("isDead", true);

        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        Debug.Log("你從太高的地方摔死了！");
    }
}
