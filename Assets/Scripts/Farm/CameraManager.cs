/* 
 * File: CameraManager.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/10/2025 
 */

using UnityEngine;

/// <summary>
/// Handles the movement of the camera based on current game states.
/// </summary>
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
            // Zooms in or out over the course of selectZoomDuration
            transitionTime += Time.deltaTime;
            float time = Mathf.Clamp01(transitionTime / selectZoomDuration);
            cameraComp.orthographicSize = Mathf.Lerp(oldOrthographicSize, targetOrthographicSize, time);
            transform.position = Vector3.Lerp(oldCameraPosition, targetCameraPosition, time);
            if (transitionTime > selectZoomDuration) selectZoom = false;
        }
    }

    /// <summary>
    /// Executed when dragging the camera.
    /// </summary>
    public void StartDrag()
    {
        startPosition = transform.position;
    }

    /// <summary>
    /// Executed when pinching the camera.
    /// </summary>
    public void StartPinch()
    {
        startZoomSize = cameraComp.orthographicSize;
    }

    /// <summary>
    /// Updates the camera location every frame.
    /// </summary>
    /// <param name="moveTo">Move vector from screen space.</param>
    public void DragCamera(Vector2 moveTo)
    {
        Vector3 calculatedMoveTo = ConvertMoveVectorByAngle(moveTo);
        transform.position = startPosition + calculatedMoveTo * dragSpeed;
    }

    /// <summary>
    /// Updates the camera zoom every frame.
    /// </summary>
    /// <param name="zoom">Zoom distance from screen space.</param>
    public void ZoomCamera(float zoom)
    {
        cameraComp.orthographicSize = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    /// <summary>
    /// Generate a Ray originating from a screen position.
    /// </summary>
    /// <param name="screenPos">A touch position on screen.</param>
    /// <returns>A ray from a screen position in the direction of the camera.</returns>
    public Ray GetRayFromScreen(Vector2 screenPos)
    {
        return cameraComp.ScreenPointToRay(screenPos);
    }

    /// <summary>
    /// Pans the camera if the currentPosition reaches within 5% of the edge of the screen.
    /// </summary>
    /// <param name="currentPosition">Position of cursor when moving an object.</param>
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

    /// <summary>
    /// Converts a 2D move vector to an XZ plane move vector in 3D (XYZ) after rotating 45 degrees in the Y axis.
    /// </summary>
    /// <param name="moveVector">Input move vector in Vector2.</param>
    /// <returns>A converted Vector3 after rotating 45 degrees in the Y axis.</returns>
    private Vector3 ConvertMoveVectorByAngle(Vector2 moveVector)
    {
        float radians = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        //Matrix multiplication when rotating 45 degrees in the Y axis.
        Vector2 rotatedMoveVector = new Vector2(
            moveVector.x * cos - moveVector.y * sin,
            moveVector.x * sin + moveVector.y * cos
        );
        return new Vector3(rotatedMoveVector.x, 0f, rotatedMoveVector.y);
    }

    /// <summary>
    /// Initiates the zoom movement to make the object on the left 25% of the screen.
    /// </summary>
    /// <param name="objectBounds">The bounds of the visible Mesh object.</param>
    public void ZoomToObjectOnLeft(Bounds objectBounds)
    {
        float maxScale = CalculateScaleFactor(objectBounds);

        // Calculate target Orthographic size
        targetOrthographicSize = cameraComp.orthographicSize / maxScale;
        oldOrthographicSize = cameraComp.orthographicSize;

        // Calculate target Camera position
        targetCameraPosition = CalculateNewCameraPosition(maxScale, objectBounds.center);
        oldCameraPosition = transform.position;

        // Setup Zoom
        transitionTime = 0f;
        selectZoom = true;
    }

    /// <summary>
    /// Zoom back to the original position and zoom size.
    /// </summary>
    public void ZoomOut()
    {
        // Reset Zoom in reverse.
        float swap = targetOrthographicSize;
        targetOrthographicSize = oldOrthographicSize;
        oldOrthographicSize = swap;
        Vector3 swapV3 = targetCameraPosition;
        targetCameraPosition = oldCameraPosition;
        oldCameraPosition = swapV3;
        transitionTime = 0f;
        selectZoom = true;
    }

    /// <summary>
    /// Calculates the necessary scale factor to make the bounds fit within the left half of the screen.
    /// </summary>
    /// <param name="objectBounds">The bounds of the mesh object.</param>
    /// <returns>A scale factor to calculate target zoom and position.</returns>
    private float CalculateScaleFactor(Bounds objectBounds)
    {
        screenSize = new Vector2(Screen.width, Screen.height);

        // Calculate the screen positions of each corner of the bounds.
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

        // Calculate the minimum X,Y and maximum X,Y of each corner.
        Vector3 min = corners[0];
        Vector3 max = corners[0];
        for (int i = 1; i < 8; i++)
        {
            min = Vector3.Min(min, corners[i]);
            max = Vector3.Max(max, corners[i]);
        }

        // Find the width and height of screen space of the object.
        float projectedWidth = max.x - min.x;
        float projectedHeight = max.y - min.y;

        // Calculate Scale Factor
        Vector2 objectSize = new Vector2(projectedWidth, projectedHeight);
        Vector2 scaleFactor = screenSize / objectSize;

        // Find the smallest value that will zoom to either the full height of the screen,
        //   or half of the width to fit within the left half of the screen.
        return Mathf.Min(scaleFactor.x / 2f, scaleFactor.y);
    }

    /// <summary>
    /// Calculates the targeted camera position post zoom.
    /// </summary>
    /// <param name="maxScale">The scale factor to potentially zoom.</param>
    /// <param name="center">The center of the object in world position.</param>
    /// <returns></returns>
    private Vector3 CalculateNewCameraPosition(float maxScale, Vector3 center)
    {
        Vector2 screenCenter = screenSize / 2f;

        // Calculate where the new object will be when zooming finishes
        Vector2 curObjectPosition = cameraComp.WorldToScreenPoint(center);
        Vector2 newObjectPosition = screenCenter + (curObjectPosition - screenCenter) * maxScale;

        // Set the new object's screen position to be the middle of the left half of the screen
        Vector2 targetScreenPosition = new Vector2(Screen.width * 0.25f, Screen.height * 0.5f);

        // Get the world positions of each spot
        Vector3 newPosition = cameraComp.ScreenToWorldPoint(newObjectPosition);
        Vector3 targetPosition = cameraComp.ScreenToWorldPoint(targetScreenPosition);

        // Calculate the offset
        Vector3 moveV = newPosition - targetPosition;

        // Scale the offset based on the zoom value
        return cameraComp.transform.position + (moveV / maxScale);
    }
}
