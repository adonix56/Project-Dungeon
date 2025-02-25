using UnityEngine;

public class BaseObjectUI : MonoBehaviour
{
    [SerializeField] protected Transform followTransform;

    protected Canvas parentCanvas;
    protected RectTransform rectTransform;
    private Camera mainCamera;
    private Vector3 startPosition;

    protected virtual void Start()
    {
        parentCanvas = transform.parent.GetComponent<Canvas>();
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Update()
    {
        if (followTransform)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(followTransform.position);
            rectTransform.anchoredPosition = Utils.GetCanvasPositionFromScreenPoint(screenPos, parentCanvas);
        }
    }

    public void SetFollowTransform(Transform followTarget)
    {
        followTransform = followTarget;
    }

    public void PublicDestroy()
    {
        Destroy(gameObject);
    }
}
