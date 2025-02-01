using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private float cameraZoomSpeed;

    private Camera cameraComp;
    private Vector3 cameraStartPosition;
    private float zoomStartSize;

    private void Start()
    {
        cameraComp = GetComponent<Camera>();
        GestureDetection.Instance.OnDragStart += DragStart;
        GestureDetection.Instance.OnDragDistance += DragCamera;
        GestureDetection.Instance.OnPinchStart += PinchStart;
        GestureDetection.Instance.OnPinchDistance += PinchDistance;
    }

    private void DragStart()
    {
        cameraStartPosition = transform.position;
    }

    private void DragCamera(Vector2 dragDistance)
    {
        dragDistance *= cameraMoveSpeed;
        dragDistance.y *= 1.2f; // Equalizing horizontal and vertical movement speed.
        transform.position = cameraStartPosition + new Vector3(dragDistance.x, 0, dragDistance.y);
    }

    private void PinchStart()
    {
        zoomStartSize = cameraComp.orthographicSize;
    }

    private void PinchDistance(float pinchDistance)
    {
        cameraComp.orthographicSize = zoomStartSize + (pinchDistance * cameraZoomSpeed);
        Debug.Log($"J$ Pinch Distance {cameraComp.orthographicSize}");
    }
}
