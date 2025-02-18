using System;
using Unity.VisualScripting;
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
    private Vector2 screenSize;
    private float startZoomSize;
    private Camera cameraComp;

    [Header("Select Zoom Transitions")]
    [SerializeField] private float selectZoomDuration;
    private float transitionTime;
    private float targetOrthographicSize;
    private float oldOrthographicSize;
    private Vector3 targetCameraPosition;
    private Vector3 oldCameraPosition;
    private bool selectZoom;

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

    private void Update()
    {
        if (selectZoom)
        {
            transitionTime += Time.deltaTime;
            float time = Mathf.Clamp01(transitionTime / selectZoomDuration);
            cameraComp.orthographicSize = Mathf.Lerp(oldOrthographicSize, targetOrthographicSize, time);
            transform.position = Vector3.Lerp(oldCameraPosition, targetCameraPosition, time);
            if (transitionTime > selectZoomDuration) selectZoom = false;
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

    public void ZoomToObjectOnLeft(Bounds objectBounds)
    {
        float maxScale = CalculateScaleFactor(objectBounds);

        targetOrthographicSize = cameraComp.orthographicSize / maxScale;
        oldOrthographicSize = cameraComp.orthographicSize;
        targetCameraPosition = CalculateNewCameraPosition(maxScale, objectBounds.center);
        oldCameraPosition = transform.position;
        transitionTime = 0f;
        selectZoom = true;

        //cameraComp.orthographicSize = targetOrthographicSize;
        //cameraComp.transform.position = targetCameraPosition;
    }

    private float CalculateScaleFactor(Bounds objectBounds)
    {
        screenSize = new Vector2(Screen.width, Screen.height);

        Vector3[] corners = new Vector3[8];
        corners[0] = objectBounds.min;
        corners[1] = new Vector3(objectBounds.max.x, objectBounds.min.y, objectBounds.min.z);
        corners[2] = new Vector3(objectBounds.min.x, objectBounds.max.y, objectBounds.min.z);
        corners[3] = new Vector3(objectBounds.min.x, objectBounds.min.y, objectBounds.max.z);
        corners[4] = new Vector3(objectBounds.max.x, objectBounds.max.y, objectBounds.min.z);
        corners[5] = new Vector3(objectBounds.max.x, objectBounds.min.y, objectBounds.max.z);
        corners[6] = new Vector3(objectBounds.max.x, objectBounds.max.y, objectBounds.max.z);
        corners[7] = objectBounds.max;

        for (int i = 0; i < 8; i++)
            corners[i] = cameraComp.WorldToScreenPoint(corners[i]);

        Vector3 min = corners[0];
        Vector3 max = corners[0];

        for (int i = 1; i < 8; i++)
        {
            min = Vector3.Min(min, corners[i]);
            max = Vector3.Max(max, corners[i]);
        }

        float projectedWidth = max.x - min.x;
        float projectedHeight = max.y - min.y;

        Vector2 objectSize = new Vector2(projectedWidth, projectedHeight);
        Vector2 scaleFactor = screenSize / objectSize;
        return Mathf.Min(scaleFactor.x / 2f, scaleFactor.y);
    }

    private Vector3 CalculateNewCameraPosition(float maxScale, Vector3 center)
    {
        Vector2 screenCenter = screenSize / 2f;

        Vector2 curObjectPosition = cameraComp.WorldToScreenPoint(center);
        Vector2 newObjectPosition = screenCenter + (curObjectPosition - screenCenter) * maxScale;

        Vector2 targetScreenPosition = new Vector2(Screen.width * 0.25f, Screen.height * 0.5f);

        Vector3 newPosition = cameraComp.ScreenToWorldPoint(newObjectPosition);
        Vector3 targetPosition = cameraComp.ScreenToWorldPoint(targetScreenPosition);

        Vector3 moveV = newPosition - targetPosition;

        return cameraComp.transform.position + (moveV / maxScale);
    }
}
