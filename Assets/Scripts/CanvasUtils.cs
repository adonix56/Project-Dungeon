using UnityEngine;

public class CanvasUtils : MonoBehaviour
{
    public static Vector2 GetCanvasPositionFromScreenPoint(Vector2 screenPoint, Canvas canvas)
    {
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        float xMultiple = rectTransform.rect.width / Screen.width;
        float yMultiple = rectTransform.rect.height / Screen.height;
        Vector2 ret = new Vector2(xMultiple * screenPoint.x, yMultiple * screenPoint.y);
        return ret;
    } 
}
