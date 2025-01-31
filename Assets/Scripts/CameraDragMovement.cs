using UnityEngine;

public class CameraDragMovement : MonoBehaviour
{
    [SerializeField] private float cameraMoveSpeed;
    private Vector3 cameraStartPosition;

    private void Start()
    {
        GestureDetection.Instance.OnDragStart += DragStart;
        GestureDetection.Instance.OnDragDistance += DragCamera;
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
}
