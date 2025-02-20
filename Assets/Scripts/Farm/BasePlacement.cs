using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BasePlacement : MonoBehaviour, ISelectable
{
    protected const string DESTROY = "Destroy";

    [SerializeField] protected SelectableSO selectableSO;
    [SerializeField] protected GameObject radialMenuPrefab;
    [SerializeField] protected GameObject confirmCancelPrefab;
    [SerializeField] protected GameObject placementSquarePrefab;
    [SerializeField] protected MeshRenderer meshRenderer;
    [SerializeField] protected Transform midpoint;
    [SerializeField] protected Vector2Int placementLocation;
    [SerializeField] protected List<Vector2Int> placementSquares;
    [SerializeField] protected Material selectorMaterial;
    [SerializeField] protected Material dimMaterial;
    [SerializeField] protected Material invalidSelectorMaterial;

    protected Animator currentRadialMenu;
    protected ConfirmCancel currentConfirmCancel;
    protected Canvas mainCanvas;
    protected Camera mainCamera;
    protected CameraManager cameraManager;
    //protected MeshRenderer mRenderer;
    protected MeshFilter mFilter;
    protected Material startMaterial;
    protected Vector3 dragOffset;
    protected GameObject oldPlacement;
    protected List<PlacementSquare> placementSquareObjects;
    protected int numInvalidSquares;


    protected virtual void Start()
    {
        mainCanvas = GameManager.Instance.GetMainCanvas();
        mainCamera = Camera.main;
        cameraManager = mainCamera.GetComponent<CameraManager>();
        mFilter = meshRenderer.gameObject.GetComponent<MeshFilter>();
        startMaterial = meshRenderer.material;
        placementSquareObjects = new List<PlacementSquare>();
        //InitializePlacement();
        SetPlaceableValues(true);
    }

    private void OnDisable()
    {
        if (placementSquareObjects != null)
        {
            foreach (PlacementSquare placementSquare in placementSquareObjects)
            {
                placementSquare.OnValidChanged -= PlacementChanged;
            }
        }
        if (currentConfirmCancel != null)
        {
            currentConfirmCancel.OnConfirm -= ConfirmMove;
            currentConfirmCancel.OnCancel -= CancelMove;
        }
    }

    private void OnEnable()
    {
        if (placementSquareObjects != null)
        {
            foreach (PlacementSquare placementSquare in placementSquareObjects)
            {
                Debug.Log("J$ BasePlacement subscribing to placementSquare OnValidChanged event");
                placementSquare.OnValidChanged += PlacementChanged;
            }
        }
        if (currentConfirmCancel != null)
        {
            currentConfirmCancel.OnConfirm += ConfirmMove;
            currentConfirmCancel.OnCancel += CancelMove;
        }
    }

    public SelectableSO Select()
    {
        //Debug.Log($"{mRenderer.bounds}");
        Bounds meshBounds = meshRenderer.bounds;
        meshBounds.center = midpoint.position;
        cameraManager.ZoomToObjectOnLeft(meshBounds);
        return selectableSO;
    }

    public void EndSelect()
    {
        cameraManager.ZoomOut();
    }

    public virtual void SelectHold()
    {
        Debug.Log("J$ BasePlacement Instantiate Radial");
        RadialFarm radialFarm = Instantiate(radialMenuPrefab, mainCanvas.transform).GetComponent<RadialFarm>();
        currentRadialMenu = radialFarm.GetComponent<Animator>();
        /*RectTransform rectTransform = radialFarm.GetComponent<RectTransform>();
        Vector3 screenPos = mainCamera.WorldToScreenPoint(UISpawnPoint.position);
        rectTransform.anchoredPosition = CanvasUtils.GetCanvasPositionFromScreenPoint(screenPos, mainCanvas);*/
        radialFarm.SetFollowTransform(midpoint);

        radialFarm.OnMove += StartMove;
    }

    public virtual void StartMove()
    {
        EndSelectHold();

        if (oldPlacement != null) Destroy(oldPlacement);

        oldPlacement = new GameObject($"{name}_Move");
        oldPlacement.transform.position = transform.position;
        oldPlacement.transform.rotation = transform.rotation;
        if (meshRenderer != null)
        {
            MeshRenderer newMRenderer = oldPlacement.AddComponent<MeshRenderer>();
            newMRenderer.sharedMaterials = meshRenderer.sharedMaterials;
            newMRenderer.material = dimMaterial;
        }
        if (mFilter != null)
        {
            MeshFilter newMFilter = oldPlacement.AddComponent<MeshFilter>();
            newMFilter.sharedMesh = mFilter.sharedMesh;
        }

        currentConfirmCancel = Instantiate(confirmCancelPrefab, mainCanvas.transform).GetComponent<ConfirmCancel>();
        //Bug fix. Set the anchored position before the next frame renders.
        RectTransform rectTransform = currentConfirmCancel.GetComponent<RectTransform>();
        Vector3 screenPos = mainCamera.WorldToScreenPoint(midpoint.position);
        rectTransform.anchoredPosition = CanvasUtils.GetCanvasPositionFromScreenPoint(screenPos, mainCanvas);
        currentConfirmCancel.SetFollowTransform(midpoint);
        meshRenderer.material = selectorMaterial;

        SetPlaceableValues(false);
        BuildPlacementSquares();

        currentConfirmCancel.OnConfirm += ConfirmMove;
        currentConfirmCancel.OnCancel += CancelMove;
        GameManager.Instance.StartMoving(this);
    }

    public virtual void Move(Vector3 newLocation, bool newDragOffset)
    {
        if (newDragOffset)
        {
            dragOffset = new Vector3(newLocation.x, transform.position.y, newLocation.z);
            dragOffset = transform.position - dragOffset;
        } else
        {
            Vector3 newPos = new Vector3(newLocation.x, transform.position.y, newLocation.z);
            newPos += dragOffset;
            newPos.x = ClosestEven(newPos.x);
            newPos.z = ClosestEven(newPos.z);
            if (transform.position != newPos)
            {
                transform.position = newPos;
                foreach (PlacementSquare placementSquare in placementSquareObjects) {
                    placementSquare.CheckColor();
                }
            }
        }
    }

    public virtual void ConfirmMove()
    {
        if (CanPlaceObject())
        {
            SetPlaceableValues(true);
            EndMove();
        }
    }

    public virtual void CancelMove()
    {
        transform.position = oldPlacement.transform.position;
        transform.rotation = oldPlacement.transform.rotation;
        SetPlaceableValues(true);
        EndMove();
    }

    public virtual void EndMove()
    {
        if (oldPlacement != null)
        {
            Destroy(oldPlacement);
            oldPlacement = null;
        }
        foreach (PlacementSquare placementSquare in placementSquareObjects)
        {
            Destroy(placementSquare.gameObject);
        }
        numInvalidSquares = 0;
        placementSquareObjects.Clear();
        meshRenderer.material = startMaterial;
        Destroy(currentConfirmCancel.gameObject);
        currentConfirmCancel = null;
        GameManager.Instance.EndMoving();
    }

    public virtual void EndSelectHold()
    {
        if (currentRadialMenu != null)
        {
            currentRadialMenu.SetTrigger(DESTROY);
            currentRadialMenu = null;
        }
    }

    private int ClosestEven(float value)
    {
        int rounded = Mathf.RoundToInt(value);
        if (rounded % 2 != 0)
        {
            if (value > rounded) return rounded + 1;
            return rounded - 1;
        }
        return rounded;
    }

    private void BuildPlacementSquares()
    {
        foreach (Vector2Int location in placementSquares)
        {
            Vector3 spawnLocation = ConvertPlacementToWorld(location);
            PlacementSquare curPlacementSquare = Instantiate(placementSquarePrefab, spawnLocation, Quaternion.identity, transform).GetComponent<PlacementSquare>();
            placementSquareObjects.Add(curPlacementSquare);
            curPlacementSquare.OnValidChanged += PlacementChanged;
        }
    }

    private void SetPlaceableValues(bool occupied)
    {
        foreach (Vector2Int location in placementSquares)
        {
            Vector3 initialPosition = ConvertPlacementToWorld(location);
            Vector2Int setSpotPosition = Vector2Int.RoundToInt(new Vector2(initialPosition.x, initialPosition.z));
            //Debug.Log($"J$ BasePlacement Setting {setSpotPosition} to {newValue}");
            FarmGrid.Instance.TrySetSpot(setSpotPosition, occupied);
        }
    }

    private Vector3 ConvertPlacementToWorld(Vector2Int location)
    {
        Vector3 retLocation = transform.position;
        retLocation.x += location.x + placementLocation.x;
        retLocation.z += location.y + placementLocation.y;
        return retLocation;
    }

    private void PlacementChanged(bool newValue)
    {
        if (newValue) numInvalidSquares--;
        else numInvalidSquares++;
        if (numInvalidSquares > 0) meshRenderer.material = invalidSelectorMaterial;
        else meshRenderer.material = selectorMaterial;
    }

    private bool CanPlaceObject() { 
        return numInvalidSquares <= 0; 
    }
}
