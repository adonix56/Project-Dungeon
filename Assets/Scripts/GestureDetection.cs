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

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraMoveSpeed;

    private PlayerController controller;
    private Vector2 startDragPosition;
    private Vector2 currentDragPosition;
    private Vector3 cameraStartPosition;
    private float startPinchDistance;
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

        controller.OnDragStart += DragStart;
        controller.OnDragEnd += DragEnd;
        controller.OnPinchStart += PinchStart;
        controller.OnPinchEnd += PinchEnd;
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

    private IEnumerator UpdateDragPosition()
    {
        while (true)
        {
            if (currentGestureType == GestureType.Drag)
            {
                currentDragPosition = controller.GetTouchPosition();
                Vector2 moveVector = (startDragPosition - currentDragPosition);
                OnDragDistance?.Invoke(moveVector);
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
