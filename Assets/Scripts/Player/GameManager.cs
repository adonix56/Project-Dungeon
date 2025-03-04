/* 
 * File: GameManager.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/05/2025 
 */

using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the overall game state and player interactions between different game components.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of GameManager
    /// </summary>
    public static GameManager Instance { get; private set; }
    /// <summary>
    /// Enumeration of possible game states.
    /// </summary>
    public enum GameState
    {
        None, Dragging, Pinching, Select, SelectHold, MovingObject, Shop
    }

    public PlayerController playerController;
    [SerializeField] private CameraManager cameraManager;
    private GameState currentGameState = GameState.None;

    [Header("Farm Stats")]
    [SerializeField] private int gardenCritChance = 10;

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
        // Singleton logic
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Connect PlayerController to game logic
        playerController = GetComponent<PlayerController>();
        playerController.OnDragStart += StartDragging;
        playerController.OnDragEnd += EndDragging;
        playerController.OnPinchStart += StartPinching;
        playerController.OnPinchEnd += EndPinching;
        playerController.OnSelectPerformed += Select;
        playerController.OnSelectHoldPerformed += SelectHold;
        farmUI = mainCanvas.GetComponent<FarmUI>();
    }

    /// <summary>
    /// The initial setup for dragging.
    /// NOTE: Placing Drag logic in Update starts pinching in the next frame. Needed for PlayerController.IsInteractingWithUI() to work.
    /// </summary>
    private void StartDragging()
    {
        // Attempt to drag unless we're moving an object.
        if (movingSelectable != null && HitSelectable(out ISelectable selectable) && selectable == movingSelectable)
        {
            currentGameState = GameState.MovingObject;
        } else
        {
            tryDragging = true;
        }
    }

    /// <summary>
    /// Ends the dragging process.
    /// </summary>
    private void EndDragging()
    {
        isDragging = false;
        newDragOffset = true;
        if (currentGameState == GameState.Dragging || currentGameState == GameState.MovingObject) ResetGameState();
    }

    /// <summary>
    /// Attempt to pinch.
    /// NOTE: Placing Pinch logic in Update starts pinching in the next frame. Needed for PlayerController.IsInteractingWithUI() to work.
    /// </summary>
    private void StartPinching()
    {
        tryPinching = true;
    }

    /// <summary>
    /// Ends the pinching process.
    /// </summary>
    private void EndPinching()
    {
        ResetGameState();
    }

    /// <summary>
    /// Initiate the moving object process.
    /// </summary>
    /// <param name="newMovingSelectable">The selectable object to move.</param>
    public void StartMoving(ISelectable newMovingSelectable)
    {
        movingSelectable = newMovingSelectable;
    }

    /// <summary>
    /// Ends the moving object process.
    /// </summary>
    public void EndMoving()
    {
        movingSelectable = null;
        ResetGameState();
    }

    /// <summary>
    /// This function is called when tapping on a selectable object, as long as no other object is already
    /// selected or the player is interacting with UI.
    /// </summary>
    private void Select()
    {
        if (playerController.IsInteractingWithUI()) return;
        if (currentGameState == GameState.Select) return;

        if (HitShop(out Shop shop))
        {
            //Remove existing Select Hold menu if exists
            EndSelectHold();

            //Setup UI
            farmUI.SetUIObjectActive(true, FarmUI.UISegment.ShopUI);
            currentGameState = GameState.Shop;
        }

        if (movingSelectable == null && HitSelectable(out ISelectable selectable))
        {
            // Remove existing Select Hold menu if exists
            EndSelectHold();

            // Set as selected object and perform Select function
            currentSelectable = selectable;
            currentSelectableSO = currentSelectable.Select();

            // Setup UI
            farmUI.SetUIObjectActive(true, FarmUI.UISegment.SelectUI, currentSelectableSO);
            currentGameState = GameState.Select;
        }
    }

    /// <summary>
    /// End the select process.
    /// </summary>
    public void EndSelect()
    {
        // Remove UI and reset select state
        farmUI.SetUIObjectActive(false, FarmUI.UISegment.SelectUI, currentSelectableSO);
        currentGameState = GameState.None;
        currentSelectable.EndSelect();
        currentSelectable = null;
        currentSelectableSO = null;
    }

    /// <summary>
    /// This function is called when tap-holding on a selectable object, as long as no other object is already
    /// selected, the player is interacting with UI, or the player is dragging on the screen.
    /// </summary>
    private void SelectHold()
    {
        if (playerController.IsInteractingWithUI()) return;
        if (currentGameState == GameState.Select) return;
        if (isDragging) return;
        if (movingSelectable == null && HitSelectable(out ISelectable selectable))
        {
            // Remove existing Select Hold menu if exists
            EndSelectHold();
            currentSelectable = selectable;
            currentSelectable.SelectHold();
            currentGameState = GameState.SelectHold;
            EndDragging();
        }
    }

    /// <summary>
    /// Ends the select hold process.
    /// </summary>
    private void EndSelectHold()
    {
        if (currentSelectable != null)
        {
            currentSelectable.EndSelectHold();
            currentSelectable = null;
            currentGameState = GameState.None;
        }
    }

    /// <summary>
    /// Resets the current game state.
    /// </summary>
    public void ResetGameState()
    {
        currentGameState = GameState.None;
    }

    private void Update()
    {
        // This method allows calculation of pinching to be deferred to the next frame, allowing 
        //    PlayerController.IsInteractingWithUI() to work correctly.
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
            // This method allows calculation of dragging to be deferred to the next frame, allowing 
            //    PlayerController.IsInteractingWithUI() to work correctly.
            if (tryDragging && !playerController.IsInteractingWithUI())
            {
                EndSelectHold();
                startDragPosition = playerController.GetTouchPosition();
                cameraManager.StartDrag();
                currentGameState = GameState.Dragging;
            }
            tryDragging = false;
        }
        else if (currentGameState == GameState.Dragging) 
        {
            MoveCamera();
        }
        else if (currentGameState == GameState.MovingObject)
        {
            MoveObject();
        }
        else if (currentGameState == GameState.Pinching)
        {
            ZoomCamera();
        }
    }

    /// <summary>
    /// Move the selectable object to follow the cursor.
    /// </summary>
    private void MoveObject()
    {
        Vector2 currentPosition = playerController.GetTouchPosition();
        // Use a raycast from the camera to the floor to find the best place to move to
        Ray ray = cameraManager.GetRayFromScreen(currentPosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, floorLayer))
        {
            movingSelectable.Move(hitInfo.point, newDragOffset);
            newDragOffset = false;
        }
        // Pan the camera if the object is moved to the edge of the screen
        cameraManager.CheckPanCamera(currentPosition);
    }

    /// <summary>
    /// Get the main canvas from the scene.
    /// </summary>
    /// <returns>The main canvas of the scene.</returns>
    public Canvas GetMainCanvas()
    {
        return mainCanvas;
    }

    /// <summary>
    /// Moves the camera based on the drag distance.
    /// </summary>
    private void MoveCamera()
    {
        Vector2 moveVector = startDragPosition - playerController.GetTouchPosition();
        if (moveVector.magnitude > minDragDistance) isDragging = true;
        moveVector.y *= 1.2f; //NOTE: Currently hard coding the equalizing, calculate based on canvas size.
        cameraManager.DragCamera(moveVector);
    }

    /// <summary>
    /// Zooms the camera based on the pinch distance.
    /// </summary>
    private void ZoomCamera()
    {
        float pinchDelta = startPinchDistance - playerController.GetPinchDistance();
        float calculatedZoom = startZoomSize + (pinchDelta * cameraZoomSpeed);
        cameraManager.ZoomCamera(calculatedZoom);
    }


    /// <summary>
    /// Detects if tap location lands on top of a Selectable.
    /// </summary>
    /// <param name="selectable">Output of selectable if hit</param>
    /// <returns>True if a selectable object was tapped on; otherwise, false.</returns>
    private bool HitSelectable(out ISelectable selectable)
    {
        Ray ray = cameraManager.GetRayFromScreen(playerController.GetTouchPosition());
        selectable = null;
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Check if object hit contains an ISelectable component.
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

    /// <summary>
    /// Detects if tap location lands on top of a Selectable.
    /// </summary>
    /// <param name="shop">Output of shop if hit</param>
    /// <returns>True if a Shop object was tapped on; otherwise, false.</returns>
    private bool HitShop(out Shop shop)
    {
        Ray ray = cameraManager.GetRayFromScreen(playerController.GetTouchPosition());
        shop = null;
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
            return hitInfo.collider.gameObject.TryGetComponent<Shop>(out shop);
        return false;
    }

    /// <summary>
    /// Performs a crit check for Garden Soil Harvesting.
    /// </summary>
    /// <returns>True if a crit was successful; otherwise, false</returns>
    public bool CheckGardenCrit()
    {
        return Random.Range(0, 99) < gardenCritChance;
    }

    /// <summary>
    /// Get the inventory component.
    /// </summary>
    /// <returns>The inventory component.</returns>
    public Inventory GetInventory()
    {
        return inventory;
    }
}
