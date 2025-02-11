using System;
using System.Collections.Generic;
using UnityEngine;

public class BasePlacement : MonoBehaviour, ISelectable
{
    protected const string DESTROY = "Destroy";

    [SerializeField] protected GameObject radialMenuPrefab;
    [SerializeField] protected GameObject confirmCancelPrefab;
    [SerializeField] protected Transform UISpawnPoint;
    [SerializeField] protected Vector2Int placementLocation;
    [SerializeField] protected List<Vector2Int> placementSquares;
    [SerializeField] protected Material selectorMaterial;
    [SerializeField] protected Material invalidSelectorMaterial;

    protected Animator currentRadialMenu;
    protected ConfirmCancel currentConfirmCancel;
    protected Canvas mainCanvas;
    protected Camera mainCamera;
    protected MeshRenderer mRenderer;
    protected Material startMaterial;
    protected Vector3 dragOffset;


    protected virtual void Start()
    {
        mainCanvas = GameManager.Instance.GetMainCanvas();
        mainCamera = Camera.main;
        mRenderer = GetComponent<MeshRenderer>();
        startMaterial = mRenderer.material;
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
        radialFarm.SetFollowTransform(UISpawnPoint);

        radialFarm.OnMove += StartMove;
    }

    public virtual void StartMove()
    {
        Debug.Log("J$ BasePlacement Move");
        EndSelect();
        currentConfirmCancel = Instantiate(confirmCancelPrefab, mainCanvas.transform).GetComponent<ConfirmCancel>();
        currentConfirmCancel.SetFollowTransform(UISpawnPoint);
        mRenderer.material = selectorMaterial;

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
            transform.position = newPos + dragOffset;
        }
    }

    public virtual void ConfirmMove()
    {
        EndMove();
    }

    public virtual void CancelMove()
    {
        EndMove();
    }

    public virtual void EndMove()
    {
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
}
