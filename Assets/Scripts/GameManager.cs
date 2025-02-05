using System.Collections;
using Unity.VisualScripting;
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
    private GestureDetection gestureDetection;
    private Camera mainCamera;
    private GameState currentGameState = GameState.None;

    [Header("Dragging")]
    [SerializeField] private float minDragDistance;
    [SerializeField] private float cameraMoveSpeed;
    private bool tryDragging = false;
    private Vector3 startCameraPosition;
    private Vector2 startDragPosition;

    [Header("Pinching")]
    [SerializeField] private float cameraZoomSpeed;
    private bool tryPinching = false;
    private float startZoomSize;
    private float startPinchDistance;


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
        playerController.OnPinchEnd += ResetGameState;
        gestureDetection = GetComponent<GestureDetection>();
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
        ResetGameState();
    }

    private void StartPinching()
    {
        tryPinching = true;
    }

    private void ResetGameState()
    {
        currentGameState = GameState.None;
    }

    private void Update()
    {
        if (tryPinching) {
            startPinchDistance = playerController.GetPinchDistance();
            startZoomSize = mainCamera.orthographicSize;
            currentGameState = GameState.Pinching;
            tryPinching = false;
        }

        if (currentGameState == GameState.None)
        {
            if (tryDragging && !EventSystem.current.IsPointerOverGameObject())
            {
                startDragPosition = playerController.GetTouchPosition();
                startCameraPosition = mainCamera.transform.position;
                currentGameState = GameState.Dragging;
                //StartCoroutine(DetectDrag());
            }
            tryDragging = false;
        }
        if (currentGameState == GameState.Dragging) 
        {
            MoveCamera();
        }
        if (currentGameState == GameState.Pinching)
        {
            ZoomCamera();
        }
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
        moveVector.y *= 1.2f; //TODO: currently hard coding the equalizing, calculate based on canvas size.
        mainCamera.transform.position = startCameraPosition + new Vector3(moveVector.x, 0, moveVector.y);
    }

    private void ZoomCamera()
    {
        float pinchDelta = startPinchDistance - playerController.GetPinchDistance();
        mainCamera.orthographicSize = startZoomSize + (pinchDelta * cameraZoomSpeed);
    }
}
