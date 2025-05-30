using UnityEngine;

public class Zipline : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Vector2 GetDirection()
    {
        return (endPoint.position - startPoint.position).normalized;
    }

    public Vector2 ClosestPoint(Vector2 playerPos)
    {
        Vector2 A = startPoint.position;
        Vector2 B = endPoint.position;

        Vector2 AP = playerPos - A;
        Vector2 AB = B - A;
        float magnitudeAB = AB.sqrMagnitude;
        float ABAPproduct = Vector2.Dot(AP, AB);
        float distance = ABAPproduct / magnitudeAB;

        distance = Mathf.Clamp01(distance);
        return A + AB * distance;
    }
}
