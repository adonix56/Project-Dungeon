using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public event Action OnDragStart;
    public event Action OnDragEnd;
    public event Action OnPinchStart;
    public event Action OnPinchEnd;
    public event Action OnSelectPerformed;
    public event Action OnSelectHoldPerformed;

    private InputControls inputControls;
    public int numTouches { get; private set; }
    private TouchID primaryTouch;

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
        inputControls.PlayerFarm.FirstContact.started += _ => TouchDown(TouchID.Drag); //StartDrag();
        inputControls.PlayerFarm.FirstContact.canceled += _ => TouchUp(TouchID.Drag); //EndDrag();
        inputControls.PlayerFarm.SecondContact.started += _ => TouchDown(TouchID.Pinch); //StartPinch();
        inputControls.PlayerFarm.SecondContact.canceled += _ => TouchUp(TouchID.Pinch); //EndPinch();

        inputControls.PlayerFarm.Select.performed += _ => Select();
        inputControls.PlayerFarm.SelectHold.performed += _ => SelectHold();
    }

    private void TouchDown(TouchID touchID)
    {
        numTouches++;
        if (numTouches == 1)
        {
            Debug.Log("J$ PlayerController StartDrag");
            primaryTouch = touchID;
            OnDragStart?.Invoke();
        } else if (numTouches == 2) {
            Debug.Log("J$ PlayerController StartPinch");
            OnPinchStart?.Invoke();
        }
    }

    private void TouchUp(TouchID touchID)
    {
        numTouches--;
        if (numTouches == 0)
        {
            Debug.Log("J$ PlayerController EndDrag");
            primaryTouch = TouchID.None;
            OnDragEnd?.Invoke();
        } else if (numTouches == 1)
        {
            Debug.Log("J$ PlayerController EndPinch StartDrag");
            primaryTouch = touchID == TouchID.Drag ? TouchID.Pinch : TouchID.Drag;
            OnPinchEnd?.Invoke();
            OnDragStart?.Invoke();
        } else
        {
            Debug.LogWarning("J$ PlayerController: TouchUp() unexpected number of touches");
        }
    }

    public Vector2 GetTouchPosition()
    {
        if (primaryTouch == TouchID.Drag)
        {
            return inputControls.PlayerFarm.FirstPosition.ReadValue<Vector2>();
        }
        return inputControls.PlayerFarm.SecondPosition.ReadValue<Vector2>();
    }

    public float GetPinchDistance()
    {
        Vector2 primaryPosition = inputControls.PlayerFarm.FirstPosition.ReadValue<Vector2>();
        Vector2 secondaryPosition = inputControls.PlayerFarm.SecondPosition.ReadValue<Vector2>();
        return Vector2.Distance(primaryPosition, secondaryPosition);
    }

    public void Select()
    {
        OnSelectPerformed?.Invoke();
    }

    public void SelectHold()
    {
        OnSelectHoldPerformed?.Invoke();
    }
}
