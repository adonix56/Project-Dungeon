/* 
 * File: PlayerController.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 01/30/2025 
 */

using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the player's input based on mobile interactions, such as Drag, Pinch,
/// Select, and Select Hold.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Event triggered when a drag action starts.
    /// </summary>
    public event Action OnDragStart;
    /// <summary>
    /// Event triggered when a drag action ends.
    /// </summary>
    public event Action OnDragEnd;
    /// <summary>
    /// Event triggered when a pinch action starts.
    /// </summary>
    public event Action OnPinchStart;
    /// <summary>
    /// Event triggered when a pinch action ends.
    /// </summary>
    public event Action OnPinchEnd;
    /// <summary>
    /// Event triggered when a select action is performed.
    /// </summary>
    public event Action OnSelectPerformed;
    /// <summary>
    /// Event triggered when a select hold action is performed.
    /// </summary>
    public event Action OnSelectHoldPerformed;
    /// <summary>
    /// Gets the number of active touches.
    /// </summary>
    public int numTouches { get; private set; }

    private InputControls inputControls;
    private TouchID primaryTouch;
    private bool isOverUI = false;

    private enum TouchID { 
        None, Drag, Pinch
    };

    private void Awake()
    {
        inputControls = new InputControls();
        numTouches = 0;
        primaryTouch = TouchID.None;
    }

    private void OnEnable()
    {
        inputControls.Enable();
    }

    private void OnDisable()
    {
        inputControls.Disable();
    }

    private void Start()
    {
        // Setup the input event handlers
        inputControls.PlayerFarm.FirstContact.started += _ => TouchDown(TouchID.Drag);
        inputControls.PlayerFarm.FirstContact.canceled += _ => TouchUp(TouchID.Drag);
        inputControls.PlayerFarm.SecondContact.started += _ => TouchDown(TouchID.Pinch);
        inputControls.PlayerFarm.SecondContact.canceled += _ => TouchUp(TouchID.Pinch);

        inputControls.PlayerFarm.Select.performed += _ => Select();
        inputControls.PlayerFarm.SelectHold.performed += _ => SelectHold();
    }

    private void Update()
    {
        isOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    /// <summary>
    /// Handles the touch down event to detect Drag or Pinch start. Currently, 
    /// only a maximum of two touches are considered.
    /// </summary>
    /// <param name="touchID">Drag on First Contact, Pinch on Second Contact.</param>
    private void TouchDown(TouchID touchID)
    {
        numTouches++;
        if (numTouches == 1)
        {
            // Setting the primaryTouch determines where to calculate GetTouchPosition().
            primaryTouch = touchID;
            OnDragStart?.Invoke();
        } else if (numTouches == 2)
        {
            OnPinchStart?.Invoke();
        }
    }

    /// <summary>
    /// Handles the touch up event to detect Drag or Pinch end. Currently,
    /// only a maximum of two touches are considered.
    /// </summary>
    /// <param name="touchID">Drag on First Contact, Pinch on Second Contact.</param>
    private void TouchUp(TouchID touchID)
    {
        numTouches--;
        if (numTouches == 0)
        {
            // Lifting last touch means no more touches
            primaryTouch = TouchID.None;
            OnDragEnd?.Invoke();
        } else if (numTouches == 1)
        {
            // When lifting FirstContact, set primaryTouch to Pinch for GetTouchPosition().
            // When lifting SecondContact, set primaryTouch to Drag for GetTouchPosition().
            primaryTouch = touchID == TouchID.Drag ? TouchID.Pinch : TouchID.Drag;
            OnPinchEnd?.Invoke();
            OnDragStart?.Invoke();
        } else
        {
            Debug.LogWarning("J$ PlayerController: TouchUp() unexpected number of touches");
        }
    }

    /// <summary>
    /// Gets the position of the current primaryTouch.
    /// </summary>
    /// <returns>The position of the current touch as a Vector2 in pixels.</returns>
    public Vector2 GetTouchPosition()
    {
        if (primaryTouch == TouchID.Drag)
        {
            return inputControls.PlayerFarm.FirstPosition.ReadValue<Vector2>();
        }
        return inputControls.PlayerFarm.SecondPosition.ReadValue<Vector2>();
    }

    /// <summary>
    /// Gets the distance between two touches during a pinch gesture.
    /// </summary>
    /// <returns>The distance between the two touch points as a float.</returns>
    public float GetPinchDistance()
    {
        Vector2 primaryPosition = inputControls.PlayerFarm.FirstPosition.ReadValue<Vector2>();
        Vector2 secondaryPosition = inputControls.PlayerFarm.SecondPosition.ReadValue<Vector2>();
        return Vector2.Distance(primaryPosition, secondaryPosition);
    }

    /// <summary>
    /// Invokes the Select event.
    /// </summary>
    public void Select()
    {
        OnSelectPerformed?.Invoke();
    }

    /// <summary>
    /// Invokes the Select Hold event.
    /// </summary>
    public void SelectHold()
    {
        OnSelectHoldPerformed?.Invoke();
    }

    /// <summary>
    /// Checks if the player is interacting with the UI.
    /// NOTE: This may check if the pointer is over UI of the previous frame.
    /// </summary>
    /// <returns>True if the player is interacting with the UI; otherwise, false.</returns>
    public bool IsInteractingWithUI()
    {
        return isOverUI;
    }
}
