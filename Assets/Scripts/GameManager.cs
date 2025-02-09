using System.Collections;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState
    {
        None, Dragging, Pinching, Select, SelectHold
    }

    private PlayerController playerController;
    //private GestureDetection gestureDetection;
    private Camera mainCamera;
    private GameState currentGameState = GameState.None;

    [Header("Dragging")]
    [SerializeField] private float minDragDistance;
    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private float angle;
    private bool tryDragging = false;
    private bool isDragging = false;
    private Vector3 startCameraPosition;
    private Vector2 startDragPosition;

    [Header("Pinching")]
    [SerializeField] private float cameraZoomSpeed;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    private bool tryPinching = false;
    private float startZoomSize;
    private float startPinchDistance;

    [Header("Selectables")]
    [SerializeField] private Canvas mainCanvas;
    private ISelectable currentSelectable;

    private ISelectable movingSelectable;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerController.OnDragStart += StartDragging;
        playerController.OnDragEnd += EndDragging;
        playerController.OnPinchStart += StartPinching;
        playerController.OnPinchEnd += EndPinching;
        playerController.OnSelectPerformed += Select;
        playerController.OnSelectHoldPerformed += SelectHold;
        //gestureDetection = GetComponent<GestureDetection>();
        mainCamera = Camera.main;
    }

    private void StartDragging()
    {
        //StopAllCoroutines();
        tryDragging = true;
    }

    private void EndDragging()
    {
        //StopAllCoroutines();
        isDragging = false;
        ResetGameState();
    }

    private void StartPinching()
    {
        tryPinching = true;
    }

    private void EndPinching()
    {
        ResetGameState();
    }

    public void StartMoving(ISelectable newMovingSelectable)
    {
        movingSelectable = newMovingSelectable;
    }

    public void EndMoving()
    {
        movingSelectable = null;
        ResetGameState();
    }

    private void Select()
    {
        if (HitInteractable(out ISelectable selectable))
        {
            EndSelect();
            currentSelectable = selectable;
            currentSelectable.Select();
            Debug.Log("J$ PlayerController Select");
        }
    }

    private void SelectHold()
    {
        if (isDragging) return;
        if (HitInteractable(out ISelectable selectable))
        {
            EndSelect();
            currentSelectable = selectable;
            currentSelectable.SelectHold();
            Debug.Log("J$ PlayerController Select Hold");
            EndDragging();
        }
    }

    private void EndSelect()
    {
        if (currentSelectable != null)
        {
            currentSelectable.EndSelect();
            currentSelectable = null;
        }
    }

    private void ResetGameState()
    {
        currentGameState = GameState.None;
    }

    private void Update()
    {
        if (tryPinching) {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                EndSelect();
                startPinchDistance = playerController.GetPinchDistance();
                startZoomSize = mainCamera.orthographicSize;
                currentGameState = GameState.Pinching;
            } else
            {
                ResetGameState();
            }
            tryPinching = false;
        }

        if (currentGameState == GameState.None)
        {
            if (tryDragging && !EventSystem.current.IsPointerOverGameObject())
            {
                EndSelect();
                startDragPosition = playerController.GetTouchPosition();
                startCameraPosition = mainCamera.transform.position;
                currentGameState = GameState.Dragging;
                //StartCoroutine(DetectDrag());
            }
            tryDragging = false;
        }
        if (currentGameState == GameState.Dragging) 
        {
            if (movingSelectable != null && HitInteractable(out ISelectable selectable) && selectable == movingSelectable) 
            { 

            } else
            {
                MoveCamera();
            }
        }
        if (currentGameState == GameState.Pinching)
        {
            ZoomCamera();
        }
    }

    public Canvas GetMainCanvas()
    {
        return mainCanvas;
    }

    public Vector2 GetCanvasPositionFromWorld(Vector3 worldPosition)
    {
        Vector3 pixelPosition = mainCamera.WorldToScreenPoint(worldPosition);
        return Vector2.zero;
    }

    private IEnumerator DetectDrag()
    {
        Vector2 moveVector = Vector2.zero;
        
        while (moveVector.magnitude < minDragDistance)
        {
            moveVector = startDragPosition - playerController.GetTouchPosition();
            yield return null;
        }
        currentGameState = GameState.Dragging;
    }

    private void MoveCamera()
    {
        Vector2 moveVector = startDragPosition - playerController.GetTouchPosition();
        moveVector *= cameraMoveSpeed;
        if (moveVector.magnitude > minDragDistance) isDragging = true;

        moveVector.y *= 1.2f; //TODO: currently hard coding the equalizing, calculate based on canvas size.
        //Rotate 45 deg
        float radians = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        //Matrix multiplication
        Vector2 rotatedMoveVector = new Vector2(
            moveVector.x * cos - moveVector.y * sin, 
            moveVector.x * sin + moveVector.y * cos
        );

        mainCamera.transform.position = startCameraPosition + new Vector3(rotatedMoveVector.x, 0, rotatedMoveVector.y);
    }

    private void ZoomCamera()
    {
        float pinchDelta = startPinchDistance - playerController.GetPinchDistance();
        float calculatedZoom = startZoomSize + (pinchDelta * cameraZoomSpeed);
        mainCamera.orthographicSize = Mathf.Clamp(calculatedZoom, minZoom, maxZoom);
    }

    private bool HitInteractable(out ISelectable selectable)
    {
        Ray ray = mainCamera.ScreenPointToRay(playerController.GetTouchPosition());
        selectable = null;
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            MonoBehaviour[] components = hitInfo.collider.gameObject.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component is ISelectable)
                {
                    selectable = (ISelectable)component;
                    return true;
                }
            }
        }
        return false;
    }
}
