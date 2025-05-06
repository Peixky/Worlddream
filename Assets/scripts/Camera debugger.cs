using UnityEngine;

public class Cameradebugger : MonoBehaviour
{
    public Camera cam;

    private void OnDrawGizmos()
    {
        if (cam == null) cam = Camera.main;

        float height = cam.orthographicSize * 2;
        float width = height * cam.aspect;

        Vector3 center = cam.transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, new Vector3(width, height, 0));
    }
}
