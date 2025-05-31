using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("幾秒內完成移動（A → B）")]
    [SerializeField] private float moveDuration = 4f;

    private float speed;
    private Transform target;

    private void Start()
    {
        target = pointB;

        // 根據 A 和 B 的距離自動計算速度
        float distance = Vector3.Distance(pointA.position, pointB.position);
        speed = distance / moveDuration;
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = target.position;

        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA;
        }

        Vector3 fixedPos = transform.position;
        fixedPos.y = Mathf.Round(fixedPos.y * 1000f) / 1000f;
        transform.position = fixedPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
