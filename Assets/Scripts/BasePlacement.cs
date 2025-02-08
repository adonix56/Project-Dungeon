using System;
using System.Collections.Generic;
using UnityEngine;

public class BasePlacement : MonoBehaviour, ISelectable
{
    protected const string DESTROY = "Destroy";

    [SerializeField] protected GameObject radialMenuPrefab;
    [SerializeField] protected Transform radialMenuSpawnPoint;

    protected Animator currentRadialMenu;
    protected Canvas mainCanvas;
    protected Camera mainCamera;

    [SerializeField] protected Vector2Int placementLocation;
    [SerializeField] protected List<Vector2Int> placementSquares;

    private void Start()
    {
        mainCanvas = GameManager.Instance.GetMainCanvas();
        mainCamera = Camera.main;
    }

    public void Select()
    {

    }

    public void SelectHold()
    {
        Debug.Log("J$ BasePlacement Instantiate Radial");
        GameObject radialMenuObject = Instantiate(radialMenuPrefab, mainCanvas.transform);
        currentRadialMenu = radialMenuObject.GetComponent<Animator>();
        RectTransform rectTransform = radialMenuObject.GetComponent<RectTransform>();
        Vector3 screenPos = mainCamera.WorldToScreenPoint(radialMenuSpawnPoint.position);
        rectTransform.anchoredPosition = CanvasUtils.GetCanvasPositionFromScreenPoint(screenPos, mainCanvas);
    }

    public void EndSelect()
    {
        if (currentRadialMenu != null)
        {
            currentRadialMenu.SetTrigger(DESTROY);
            currentRadialMenu = null;
        }
    }
}
