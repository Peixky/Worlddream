using UnityEngine;
using System.Collections;

public class MovingPlatformController : MonoBehaviour
{
    [Header("移動設定")]
    public Vector3[] waypoints; 
    public float moveSpeed = 2f; 
    public float waitAtWaypointDuration = 1f; 

    public float waitAtZeroDuration = 2f; 

    public ButtonController linkedButton; 

    private bool isMoving = false; 

    void Start()
    {
        if (waypoints.Length < 2)
        {
            enabled = false;
            return;
        }

        
        transform.position = waypoints[0];
    }

    public void StartPlatformMoveFromZero()
    {
        if (isMoving) return; 
        isMoving = true;
        StopAllCoroutines();
        StartCoroutine(MoveToWaypointsRoutine());
    }

    IEnumerator MoveToWaypointsRoutine()
    {
        Vector3 startPoint;
        Vector3 targetPointZero;
        float currentMoveTime;
        float totalDistance;
        float durationToZero;

        Vector3 startPointOne;
        Vector3 targetPointOne;
        float currentMoveTimeOne;
        float totalDistanceOne;
        float durationToOne;

        // 1. 移動到第一個路徑點 (Element 0)
        startPoint = transform.position;
        targetPointZero = waypoints[0]; 

        // 如果平台當前不在 Element 0，則移動過去
        if (Vector3.Distance(startPoint, targetPointZero) > 0.05f)
        {
            currentMoveTime = 0f; 
            totalDistance = Vector3.Distance(startPoint, targetPointZero); 
            durationToZero = totalDistance / moveSpeed; 

            while (currentMoveTime < durationToZero)
            {
                transform.position = Vector3.Lerp(startPoint, targetPointZero, currentMoveTime / durationToZero);
                currentMoveTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPointZero; 
        }

        
        yield return new WaitForSeconds(waitAtZeroDuration);

        // 2. 移動到第二個路徑點 (Element 1)
        startPointOne = transform.position; 
        targetPointOne = waypoints[1]; 

        currentMoveTimeOne = 0f;
        totalDistanceOne = Vector3.Distance(startPointOne, targetPointOne); 
        durationToOne = totalDistanceOne / moveSpeed; 

        while (currentMoveTimeOne < durationToOne)
        {
            transform.position = Vector3.Lerp(startPointOne, targetPointOne, currentMoveTimeOne / durationToOne);
            currentMoveTimeOne += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPointOne; 

       
        yield return new WaitForSeconds(waitAtWaypointDuration); 

        // 3. 自動返回第一個路徑點 (Element 0)
        startPoint = transform.position; 
        targetPointZero = waypoints[0]; 
        currentMoveTime = 0f; 
        totalDistance = Vector3.Distance(startPoint, targetPointZero); 
        durationToZero = totalDistance / moveSpeed; 

        while (currentMoveTime < durationToZero)
        {
            transform.position = Vector3.Lerp(startPoint, targetPointZero, currentMoveTime / durationToZero);
            currentMoveTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPointZero; // 確保精確回到 Element 0

       
        isMoving = false; 

        // 通知連結的按鈕恢復圖片
        if (linkedButton != null)
        {
            linkedButton.ResetButton();
        }
    }

    // 在 Scene 視窗中可視化路徑點
    void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.DrawSphere(waypoints[i], 0.2f); 
            if (i > 0)
            {
                Gizmos.DrawLine(waypoints[i - 1], waypoints[i]);
            }
        }
    }
}