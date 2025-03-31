/* 
 * File: BasePlacement.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/08/2025 
 */

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for farm placement objects.
/// </summary>
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
    protected FarmUI farmUI;
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
        farmUI = mainCanvas.GetComponent<FarmUI>();
        mainCamera = Camera.main;
        cameraManager = mainCamera.GetComponent<CameraManager>();
        mFilter = meshRenderer.gameObject.GetComponent<MeshFilter>();
        startMaterial = meshRenderer.material;
        placementSquareObjects = new List<PlacementSquare>();
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

    /// <summary>
    /// Executed when object is selected.
    /// </summary>
    /// <returns>The selectable's corresponding scriptable object.</returns>
    public virtual SelectableSO Select()
    {
        Bounds meshBounds = meshRenderer.bounds;
        meshBounds.center = midpoint.position;
        cameraManager.ZoomToObjectOnLeft(meshBounds);
        return selectableSO;
    }

    /// <summary>
    /// Executed when select process is ended.
    /// </summary>
    public virtual void EndSelect()
    {
        cameraManager.ZoomOut();
    }

    /// <summary>
    /// Executed when object is select holded.
    /// </summary>
    public virtual void SelectHold()
    {
        RadialFarm radialFarm = Instantiate(radialMenuPrefab, mainCanvas.transform).GetComponent<RadialFarm>();
        currentRadialMenu = radialFarm.GetComponent<Animator>();
        radialFarm.SetFollowTransform(midpoint);

        radialFarm.OnMove += StartMove;
    }

    /// <summary>
    /// Executed when attempting to move the object.
    /// </summary>
    public virtual void StartMove()
    {
        EndSelectHold();

        // Create copy of current instance to show its current placement
        if (oldPlacement != null) Destroy(oldPlacement);
        oldPlacement = new GameObject($"{name}_Move");
        // Copy the transform values
        oldPlacement.transform.position = transform.position;
        oldPlacement.transform.rotation = transform.rotation;
        // Copy the visuals
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

        // Bug fix. Set the anchored position before the next frame renders.
        RectTransform rectTransform = currentConfirmCancel.GetComponent<RectTransform>();
        Vector3 screenPos = mainCamera.WorldToScreenPoint(midpoint.position);
        rectTransform.anchoredPosition = Utils.GetCanvasPositionFromScreenPoint(screenPos, mainCanvas);

        // Set object to follow cursor
        currentConfirmCancel.SetFollowTransform(midpoint);
        meshRenderer.material = selectorMaterial;

        // Display viable placement squares
        SetPlaceableValues(false);
        BuildPlacementSquares();

        currentConfirmCancel.OnConfirm += ConfirmMove;
        currentConfirmCancel.OnCancel += CancelMove;
        GameManager.Instance.StartMoving(this);
    }

    /// <summary>
    /// Move object to accepted new location.
    /// </summary>
    /// <param name="newLocation">New location to move the object</param>
    /// <param name="newDragOffset">True sets up the offset from the location to the cursor, False to change the location of the object.</param>
    public virtual void Move(Vector3 newLocation, bool newDragOffset)
    {
        if (newDragOffset) // Set the offset from the object to the cursor
        {
            dragOffset = new Vector3(newLocation.x, transform.position.y, newLocation.z);
            dragOffset = transform.position - dragOffset;
        } else // Move the object to the cursor location
        {
            Vector3 newPos = new Vector3(newLocation.x, transform.position.y, newLocation.z);
            newPos += dragOffset;
            // Grid squares are 2x2 so find the closest even position.
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

    /// <summary>
    /// Permanently move the farm object if there are no red squares.
    /// </summary>
    public virtual void ConfirmMove()
    {
        if (CanPlaceObject())
        {
            SetPlaceableValues(true);
            EndMove();
        }
    }

    /// <summary>
    /// Move the current location to the previously stored location and rotation.
    /// </summary>
    public virtual void CancelMove()
    {
        transform.position = oldPlacement.transform.position;
        transform.rotation = oldPlacement.transform.rotation;
        SetPlaceableValues(true);
        EndMove();
    }

    /// <summary>
    /// Ends the move process and performs cleanup of extraneous objects.
    /// </summary>
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

    /// <summary>
    /// Executed when ending select hold.
    /// </summary>
    public virtual void EndSelectHold()
    {
        if (currentRadialMenu != null)
        {
            currentRadialMenu.SetTrigger(DESTROY);
            currentRadialMenu = null;
        }
    }

    /// <summary>
    /// Calculates the closest even integer.
    /// </summary>
    /// <param name="value">Float value to find the closest even integer.</param>
    /// <returns>The closest even integer.</returns>
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

    /// <summary>
    /// Spawns placement squares based on the spawn locations.
    /// </summary>
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

    /// <summary>
    /// Registers the current placement spawn locations as occupied to the FarmGrid.
    /// </summary>
    /// <param name="occupied"></param>
    private void SetPlaceableValues(bool occupied)
    {
        foreach (Vector2Int location in placementSquares)
        {
            Vector3 initialPosition = ConvertPlacementToWorld(location);
            Vector2Int setSpotPosition = Vector2Int.RoundToInt(new Vector2(initialPosition.x, initialPosition.z));
            FarmGrid.Instance.TrySetSpot(setSpotPosition, occupied);
        }
    }

    /// <summary>
    /// Converts placement spawn local coordinates to world coordinates.
    /// </summary>
    /// <param name="location">Location to convert to world coordinates in Vector2Int</param>
    /// <returns>The Vector3 world position of the converted position.</returns>
    private Vector3 ConvertPlacementToWorld(Vector2Int location)
    {
        Vector3 retLocation = transform.position;
        retLocation.x += location.x + placementLocation.x;
        retLocation.z += location.y + placementLocation.y;
        return retLocation;
    }

    /// <summary>
    /// Executed when a placement square has changed its value.
    /// </summary>
    /// <param name="newValue">True for a valid square, False for an invalid square.</param>
    private void PlacementChanged(bool newValue)
    {
        if (newValue) numInvalidSquares--;
        else numInvalidSquares++;
        if (numInvalidSquares > 0) meshRenderer.material = invalidSelectorMaterial;
        else meshRenderer.material = selectorMaterial;
    }

    /// <summary>
    /// Determines whether the object can be placed in the current location.
    /// </summary>
    /// <returns>True if the object can be placed; other, false.</returns>
    private bool CanPlaceObject() { 
        return numInvalidSquares <= 0; 
    }
}
