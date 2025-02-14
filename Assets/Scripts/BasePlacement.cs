using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BasePlacement : MonoBehaviour, ISelectable
{
    protected const string DESTROY = "Destroy";

    [SerializeField] protected GameObject radialMenuPrefab;
    [SerializeField] protected GameObject confirmCancelPrefab;
    [SerializeField] protected GameObject placementSquarePrefab;
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
    protected MeshRenderer mRenderer;
    protected MeshFilter mFilter;
    protected Material startMaterial;
    protected Vector3 dragOffset;
    protected GameObject oldPlacement;
    protected List<PlacementSquare> placementSquareObjects;
    protected int numValidSquares;


    protected virtual void Start()
    {
        mainCanvas = GameManager.Instance.GetMainCanvas();
        mainCamera = Camera.main;
        mRenderer = GetComponent<MeshRenderer>();
        mFilter = GetComponent<MeshFilter>();
        startMaterial = mRenderer.material;
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

    public virtual void Select()
    {
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
        EndSelect();
        
        if (oldPlacement != null) Destroy(oldPlacement);

        oldPlacement = new GameObject($"{name}_Move");
        oldPlacement.transform.position = transform.position;
        oldPlacement.transform.rotation = transform.rotation;
        if (mRenderer != null)
        {
            MeshRenderer newMRenderer = oldPlacement.AddComponent<MeshRenderer>();
            newMRenderer.sharedMaterials = mRenderer.sharedMaterials;
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
        mRenderer.material = selectorMaterial;

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
        EndMove();
    }

    public virtual void CancelMove()
    {
        transform.position = oldPlacement.transform.position;
        transform.rotation = oldPlacement.transform.rotation;
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
        placementSquareObjects.Clear();
        mRenderer.material = startMaterial;
        Destroy(currentConfirmCancel.gameObject);
        currentConfirmCancel = null;
        GameManager.Instance.EndMoving();
    }

    public virtual void EndSelect()
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

    private void SetPlaceableValues(bool newValue)
    {
        foreach (Vector2Int location in placementSquares)
        {
            Vector3 initialPosition = ConvertPlacementToWorld(location);
            Vector2Int setSpotPosition = Vector2Int.RoundToInt(new Vector2(initialPosition.x, initialPosition.z));
            //Debug.Log($"J$ BasePlacement Setting {setSpotPosition} to {newValue}");
            FarmGrid.Instance.TrySetSpot(setSpotPosition, newValue);
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
        if (newValue) numValidSquares--;
        else numValidSquares++;
        if (numValidSquares > 0) mRenderer.material = invalidSelectorMaterial;
        else mRenderer.material = selectorMaterial;
    }
}
