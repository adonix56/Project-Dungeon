using System;
using System.Collections;
using UnityEngine;

public class GestureDetection : MonoBehaviour
{
    public static GestureDetection Instance;

    public enum GestureType
    {
        None, Tap, Hold, Drag, Pinch
    }

    public event Action OnDragStart;
    public event Action<Vector2> OnDragDistance;
    public event Action OnPinchStart;
    public event Action<float> OnPinchDistance;
    //public event Action<ISelectable> OnSelect;
    //public event Action<ISelectable> OnSelectHold;

    [SerializeField] private float minDragDistance;

    private PlayerController controller;
    private Camera mainCamera;
    private Vector2 startDragPosition;
    private float startPinchDistance;
    private bool isDragging;
    public GestureType currentGestureType { get; private set; }

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
        currentGestureType = GestureType.None;
        controller = GetComponent<PlayerController>();
        mainCamera = Camera.main;

        controller.OnDragStart += DragStart;
        controller.OnDragEnd += DragEnd;
        controller.OnPinchStart += PinchStart;
        controller.OnPinchEnd += PinchEnd;
        controller.OnSelectPerformed += Select;
        controller.OnSelectHoldPerformed += SelectHold;
    }

    private void DragStart()
    {
        // 10ms delay for race condition for touch activation and touch position.
        Invoke("SetDragStartPosition", 0.01f);
    }

    private void SetDragStartPosition()
    {
        OnDragStart?.Invoke();
        currentGestureType = GestureType.Drag;
        startDragPosition = controller.GetTouchPosition();
        StartCoroutine(UpdateDragPosition());
    }

    private void DragEnd()
    {
        StopAllCoroutines();
        isDragging = false;
        startDragPosition = Vector2.zero;
        currentGestureType = GestureType.None;
    }

    private void PinchStart()
    {
        // 10ms delay for race condition for touch activation and touch position.
        Invoke("SetPinchStart", 0.01f);
    }

    private void SetPinchStart()
    {
        currentGestureType = GestureType.Pinch;
        startPinchDistance = controller.GetPinchDistance();
        OnPinchStart?.Invoke();
        StopAllCoroutines();
        StartCoroutine(UpdatePinchDistance());
    }

    private void PinchEnd()
    {
        StopAllCoroutines();
        currentGestureType = GestureType.None;
    }

    private void Select()
    {
        if (HitInteractable(out ISelectable selectable))
        {
            selectable.Select();
            Debug.Log("J$ GestureDetection Select");
            DragEnd();
        }
    }

    private void SelectHold()
    {
        if (!isDragging && HitInteractable(out ISelectable selectable)) 
        {
            selectable.SelectHold();
            Debug.Log("J$ GestureDetection SelectHold");
            DragEnd();
        }
    }

    private bool HitInteractable(out ISelectable selectable)
    {
        Ray ray = mainCamera.ScreenPointToRay(controller.GetTouchPosition());
        selectable = null;
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            MonoBehaviour[] components = hitInfo.collider.gameObject.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component is ISelectable)
                {
                    selectable = component as ISelectable;
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator UpdateDragPosition()
    {
        while (true)
        {
            if (currentGestureType == GestureType.Drag)
            {
                Vector2 currentDragPosition = controller.GetTouchPosition();
                Vector2 moveVector = (startDragPosition - currentDragPosition);
                if (moveVector.magnitude > minDragDistance) isDragging = true;
                if (isDragging) OnDragDistance?.Invoke(moveVector);
            }
            yield return null;
        }
    }

    private IEnumerator UpdatePinchDistance()
    {
        while (true)
        {
            if (currentGestureType == GestureType.Pinch)
            {
                OnPinchDistance?.Invoke(startPinchDistance - controller.GetPinchDistance());
            }
            yield return null;
        }
    }
}
