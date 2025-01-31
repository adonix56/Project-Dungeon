using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public event Action OnDragStart;
    public event Action OnDragEnd;
    public event Action OnPinchStart;
    public event Action OnPinchEnd;

    private InputControls inputControls;

    private void Awake()
    {
        inputControls = new InputControls();
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
        inputControls.PlayerFarm.DragContact.started += _ => StartDrag();
        inputControls.PlayerFarm.DragContact.canceled += _ => EndDrag();
        inputControls.PlayerFarm.PinchContact.started += _ => StartPinch();
        inputControls.PlayerFarm.PinchContact.canceled += _ => EndPinch();
    }

    private void StartDrag()
    {
        Debug.Log("StartDrag");
        OnDragStart?.Invoke();
    }

    private void EndDrag()
    {
        Debug.Log("EndDrag");
        OnDragEnd?.Invoke();
    }

    private void StartPinch()
    {
        Debug.Log("StartPinch");
        OnPinchStart?.Invoke();
    }

    private void EndPinch()
    {
        Debug.Log("EndPinch");
        OnPinchEnd?.Invoke();
    }

    public Vector2 GetTouchPosition()
    {
        return inputControls.PlayerFarm.DragPosition.ReadValue<Vector2>();
    }

    public Vector2 GetPinchPosition()
    {
        return inputControls.PlayerFarm.PinchPosition.ReadValue<Vector2>();
    }
}
