using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("幾秒內完成移動（A → B）")]
    [SerializeField] private float moveDuration = 4f;

    private float speed;
    private Transform target;
    private Vector3 lastPosition;

    public Vector3 PlatformVelocity { get; private set; }

    private void Start()
    {
        target = pointB;
        float distance = Vector3.Distance(pointA.position, pointB.position);
        speed = distance / moveDuration;

        lastPosition = transform.position;
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

        PlatformVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        Vector3 fixedPos = transform.position;
        fixedPos.y = Mathf.Round(fixedPos.y * 1000f) / 1000f;
        transform.position = fixedPos;
    }
}
