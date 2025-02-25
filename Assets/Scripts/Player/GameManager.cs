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
        None, Dragging, Pinching, Select, SelectHold, MovingObject
    }

    private PlayerController playerController;
    [SerializeField] private CameraManager cameraManager;
    private GameState currentGameState = GameState.None;

    [Header("Dragging")]
    [SerializeField] private float minDragDistance;
    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private float angle;
    private bool tryDragging = false;
    private bool isDragging = false;
    private bool newDragOffset = true;
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
    [SerializeField] private LayerMask floorLayer;
    private FarmUI farmUI;
    private ISelectable currentSelectable;
    private SelectableSO currentSelectableSO;
    private ISelectable movingSelectable;

    [Header("Inventory")]
    [SerializeField] private Inventory inventory;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        farmUI = mainCanvas.GetComponent<FarmUI>();
    }

    private void StartDragging()
    {
        if (movingSelectable != null && HitInteractable(out ISelectable selectable) && selectable == movingSelectable)
        {
            currentGameState = GameState.MovingObject;
        } else
        {
            tryDragging = true;
        }
    }

    private void EndDragging()
    {
        isDragging = false;
        newDragOffset = true;
        if (currentGameState == GameState.Dragging || currentGameState == GameState.MovingObject) ResetGameState();
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
        if (playerController.IsInteractingWithUI()) return;
        if (currentGameState == GameState.Select) return;
        if (movingSelectable == null && HitInteractable(out ISelectable selectable))
        {
            EndSelectHold();
            currentSelectable = selectable;
            currentSelectableSO = currentSelectable.Select();
            farmUI.SetUIObjectActive(true, FarmUI.UIObject.SelectUI, currentSelectableSO);
            currentGameState = GameState.Select;
            Debug.Log("J$ PlayerController Select");
        }
    }

    public void EndSelect()
    {
        farmUI.SetUIObjectActive(false, FarmUI.UIObject.SelectUI, currentSelectableSO);
        currentGameState = GameState.None;
        currentSelectable.EndSelect();
        currentSelectable = null;
        currentSelectableSO = null;
    }

    private void SelectHold()
    {
        if (playerController.IsInteractingWithUI()) return;
        if (currentGameState == GameState.Select) return;
        if (isDragging) return;
        if (movingSelectable == null && HitInteractable(out ISelectable selectable))
        {
            EndSelectHold();
            currentSelectable = selectable;
            currentSelectable.SelectHold();
            currentGameState = GameState.SelectHold;
            Debug.Log("J$ PlayerController Select Hold");
            EndDragging();
        }
    }

    private void EndSelectHold()
    {
        if (currentSelectable != null)
        {
            Debug.Log("J$ GameManager EndSelect from GM");
            currentSelectable.EndSelectHold();
            currentSelectable = null;
            currentGameState = GameState.None;
        }
    }

    private void ResetGameState()
    {
        currentGameState = GameState.None;
    }

    private void Update()
    {
        if (tryPinching) {
            if (!playerController.IsInteractingWithUI())
            {
                EndSelectHold();
                startPinchDistance = playerController.GetPinchDistance();
                cameraManager.StartPinch();
                currentGameState = GameState.Pinching;
            } else
            {
                ResetGameState();
            }
            tryPinching = false;
        }

        if (currentGameState == GameState.None)
        {
            if (tryDragging && !playerController.IsInteractingWithUI())
            {
                EndSelectHold();
                startDragPosition = playerController.GetTouchPosition();
                cameraManager.StartDrag();
                currentGameState = GameState.Dragging;
                //StartCoroutine(DetectDrag());
            }
            tryDragging = false;
        }
        if (currentGameState == GameState.Dragging) 
        {
            MoveCamera();
        }
        if (currentGameState == GameState.MovingObject)
        {
            MoveObject();
        }
        if (currentGameState == GameState.Pinching)
        {
            ZoomCamera();
        }
    }

    private void MoveObject()
    {
        Vector2 currentPosition = playerController.GetTouchPosition();
        Ray ray = cameraManager.GetRayFromScreen(currentPosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, floorLayer))
        {
            movingSelectable.Move(hitInfo.point, newDragOffset);
            newDragOffset = false;
        }
        cameraManager.CheckPanCamera(currentPosition);
    }

    public Canvas GetMainCanvas()
    {
        return mainCanvas;
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
        if (moveVector.magnitude > minDragDistance) isDragging = true;
        moveVector.y *= 1.2f; //TODO: currently hard coding the equalizing, calculate based on canvas size.
        cameraManager.DragCamera(moveVector);
    }

    private void ZoomCamera()
    {
        float pinchDelta = startPinchDistance - playerController.GetPinchDistance();
        float calculatedZoom = startZoomSize + (pinchDelta * cameraZoomSpeed);
        cameraManager.ZoomCamera(calculatedZoom);
    }

    private bool HitInteractable(out ISelectable selectable)
    {
        Ray ray = cameraManager.GetRayFromScreen(playerController.GetTouchPosition());
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

    public Inventory GetInventory()
    {
        return inventory;
    }
}
