using UnityEngine;
using System.Collections;

public class MovingPlatformController : MonoBehaviour
{
    [Header("移動設定")]
    public Vector3[] waypoints; // 平台移動路徑點，Element 0 是起始點，Element 1 是目標點
    public float moveSpeed = 2f; // 平台移動速度
    public float waitAtWaypointDuration = 1f; // 到達 Element 1 後等待時間

    // 新增：壓下按鈕後，平台在 Element 0 等待的時間
    public float waitAtZeroDuration = 2f; 

    // 新增：連結按鈕控制器，以便在平台恢復後通知按鈕恢復圖片
    public ButtonController linkedButton; 

    private bool isMoving = false; // 平台是否正在移動

    void Start()
    {
        if (waypoints.Length < 2)
        {
            //Debug.LogWarning("MovingPlatformController: 路徑點不足，請設定至少兩個路徑點。", this);
            enabled = false;
            return;
        }

        // 確保平台從第一個路徑點開始
        transform.position = waypoints[0];
    }

    // 由 ButtonController 呼叫的方法，開始平台的精確移動流程
    public void StartPlatformMoveFromZero()
    {
        if (isMoving) return; // 如果平台已經在移動中，則避免重複啟動
        isMoving = true;
        StopAllCoroutines(); // 停止任何可能正在運行的舊協程
        StartCoroutine(MoveToWaypointsRoutine());
    }

    IEnumerator MoveToWaypointsRoutine()
    {
        // **在協程開頭一次性宣告所有需要用到的變數**
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
        startPoint = transform.position; // 賦值
        targetPointZero = waypoints[0]; // 賦值

        // 如果平台當前不在 Element 0，則移動過去
        if (Vector3.Distance(startPoint, targetPointZero) > 0.05f)
        {
            currentMoveTime = 0f; // 賦值
            totalDistance = Vector3.Distance(startPoint, targetPointZero); // 賦值
            durationToZero = totalDistance / moveSpeed; // 賦值

            while (currentMoveTime < durationToZero)
            {
                transform.position = Vector3.Lerp(startPoint, targetPointZero, currentMoveTime / durationToZero);
                currentMoveTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPointZero; // 確保精確到達
        }

        //Debug.Log(gameObject.name + " 已到達 Element 0，等待 " + waitAtZeroDuration + " 秒。");
        yield return new WaitForSeconds(waitAtZeroDuration); // 在 Element 0 等待指定時間 (2 秒)

        // 2. 移動到第二個路徑點 (Element 1)
        startPointOne = transform.position; // 賦值
        targetPointOne = waypoints[1]; // 賦值

        currentMoveTimeOne = 0f; // 賦值
        totalDistanceOne = Vector3.Distance(startPointOne, targetPointOne); // 賦值
        durationToOne = totalDistanceOne / moveSpeed; // 賦值

        while (currentMoveTimeOne < durationToOne)
        {
            transform.position = Vector3.Lerp(startPointOne, targetPointOne, currentMoveTimeOne / durationToOne);
            currentMoveTimeOne += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPointOne; // 確保精確到達 Element 1

        //Debug.Log(gameObject.name + " 已到達 Element 1。等待 " + waitAtWaypointDuration + " 秒。");
        yield return new WaitForSeconds(waitAtWaypointDuration); // 在 Element 1 等待時間

        // 3. 自動返回第一個路徑點 (Element 0)
        startPoint = transform.position; // 重新賦值
        targetPointZero = waypoints[0]; // 重新賦值
        currentMoveTime = 0f; // 重新賦值
        totalDistance = Vector3.Distance(startPoint, targetPointZero); // 重新賦值
        durationToZero = totalDistance / moveSpeed; // 重新賦值

        while (currentMoveTime < durationToZero)
        {
            transform.position = Vector3.Lerp(startPoint, targetPointZero, currentMoveTime / durationToZero);
            currentMoveTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPointZero; // 確保精確回到 Element 0

        //Debug.Log(gameObject.name + " 已回到 Element 0。");
        isMoving = false; // 標記為停止移動

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
            Gizmos.DrawSphere(waypoints[i], 0.2f); // 繪製路徑點
            if (i > 0)
            {
                Gizmos.DrawLine(waypoints[i - 1], waypoints[i]); // 繪製路徑線
            }
        }
    }
}