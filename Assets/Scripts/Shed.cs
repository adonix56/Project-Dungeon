using UnityEngine;

public class Shed : MonoBehaviour, ISelectable
{
    private const string DESTROY = "Destroy";

    [SerializeField] private GameObject radialMenuPrefab;
    [SerializeField] private Transform radialMenuSpawnPoint;

    private Animator currentRadialMenu;
    private Canvas mainCanvas;
    private Camera mainCamera;

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
        Debug.Log("J$ Shed Instantiate Radial");
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
