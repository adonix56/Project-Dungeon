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

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraMoveSpeed;

    private PlayerController controller;
    private bool isDragging;
    private Vector2 startDragPosition;
    private Vector2 currentDragPosition;
    private Vector3 cameraStartPosition;
    private Coroutine dragCoroutine;
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
    }

    private void DragStart()
    {
        // 10ms delay for race condition for touch activation and touch position.
        Invoke("SetStartPosition", 0.01f);
    }

    private void SetStartPosition()
    {
        OnDragStart?.Invoke();
        currentGestureType = GestureType.Drag;
        startDragPosition = controller.GetTouchPosition();
        dragCoroutine = StartCoroutine(UpdateDragPosition());
    }

    private void DragEnd()
    {
        StopCoroutine(dragCoroutine);
        startDragPosition = Vector2.zero;
        currentGestureType = GestureType.None;
    }

    private IEnumerator UpdateDragPosition()
    {
        while (true)
        {
            currentDragPosition = controller.GetTouchPosition();
            Vector2 moveVector = (startDragPosition - currentDragPosition);
            OnDragDistance?.Invoke(moveVector);
            yield return null;
        }
    }
}
