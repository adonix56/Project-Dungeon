using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float dragSpeed;
    [SerializeField] private float panningSpeed;
    [SerializeField] private float minPanningSize;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private float angle;
    private Vector3 startPosition;
    private float startZoomSize;
    private Camera cameraComp;

    private void Start()
    {
        cameraComp = GetComponent<Camera>();
        if (minZoom > maxZoom)
        {
            float temp = maxZoom;
            maxZoom = minZoom;
            minZoom = temp;
        }
    }

    public void StartDrag()
    {
        startPosition = transform.position;
    }

    public void StartPinch()
    {
        startZoomSize = cameraComp.orthographicSize;
    }

    public void DragCamera(Vector2 moveTo)
    {
        Vector3 calculatedMoveTo = ConvertMoveVectorByAngle(moveTo);
        transform.position = startPosition + calculatedMoveTo * dragSpeed;
    }

    public void ZoomCamera(float zoom)
    {
        cameraComp.orthographicSize = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    public Ray GetRayFromScreen(Vector2 screenPos)
    {
        return cameraComp.ScreenPointToRay(screenPos);
    }

    public void CheckPanCamera(Vector2 currentPosition)
    {
        float xMax = Screen.width;
        float yMax = Screen.height;
        Vector2 panVector = Vector2.zero;
        if (currentPosition.x < xMax * 0.05 || currentPosition.x < minPanningSize)
            panVector.x = -1f; // pan left
        else if (currentPosition.x > xMax * 0.95 || currentPosition.x > xMax - minPanningSize)
            panVector.x = 1f; // pan right
        if (currentPosition.y < yMax * 0.05 || currentPosition.y < minPanningSize)
            panVector.y = -1f; // pan down
        else if (currentPosition.y > yMax * 0.95 || currentPosition.y > yMax - minPanningSize)
            panVector.y = 1f; // pan up
        panVector = panVector.normalized * Time.deltaTime * panningSpeed;
        Vector3 calculatedPanVector = ConvertMoveVectorByAngle(panVector);
        transform.position += calculatedPanVector;
    }

    private Vector3 ConvertMoveVectorByAngle(Vector2 moveVector)
    {
        float radians = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        //Matrix multiplication
        Vector2 rotatedMoveVector = new Vector2(
            moveVector.x * cos - moveVector.y * sin,
            moveVector.x * sin + moveVector.y * cos
        );
        return new Vector3(rotatedMoveVector.x, 0f, rotatedMoveVector.y);
    }
}
