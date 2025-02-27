/* 
 * File: BaseObjectUI.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/19/2025 
 */

using UnityEngine;

/// <summary>
/// Dynamic UI objects placed in the world.
/// </summary>
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

    /// <summary>
    /// Makes the UI object follow a specific object.
    /// </summary>
    /// <param name="followTarget">Object to follow.</param>
    public void SetFollowTransform(Transform followTarget)
    {
        followTransform = followTarget;
    }

    /// <summary>
    /// A public function to destroy itself. Allows destruction of object in Animator.
    /// </summary>
    public void PublicDestroy()
    {
        Destroy(gameObject);
    }
}
