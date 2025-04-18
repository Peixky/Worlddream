using UnityEngine;

public class Playermovement : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody2D body;
  
    private void Awake(){
        body = GetComponent<Rigidbody2D>();
    }

    private void Update(){
        float  horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.linearVelocity.y);//在y值不變的情況下紀錄x
        
        //Flip player when moving left-right
        if(horizontalInput > 0.01f){
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }else if(horizontalInput < -0.01f){
            Vector3 scale = transform.localScale;
            scale.x = horizontalInput > 0.01f ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        if(Input.GetKey(KeyCode.Space)){
            body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
        }
    }
}
