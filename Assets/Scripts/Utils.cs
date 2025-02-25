using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct InventoryItem
{
    public int quality;
    public int quantity;
}

public enum InventoryCategory
{
    Gold, Harvest, Seeds
}

[Serializable]
public class InventoryKeyValuePair
{
    public InventoryItemSO key;
    public List<InventoryItem> value; 
}

[Serializable]
public class InventoryDictionaryWrapper
{
    public List<InventoryKeyValuePair> keyValuePairs = new List<InventoryKeyValuePair>();
}

[Serializable]
public class ItemsWrapper
{
    public List<Tuple<InventoryItemSO, InventoryItem>> items = new List<Tuple<InventoryItemSO, InventoryItem>>();
    public int Count { get { return items.Count; } }
    public Tuple<InventoryItemSO, InventoryItem> Get(int index) {  return items[index]; }
}

public class Utils : MonoBehaviour
{
    public static Vector2 GetCanvasPositionFromScreenPoint(Vector2 screenPoint, Canvas canvas)
    {
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        float xMultiple = rectTransform.rect.width / Screen.width;
        float yMultiple = rectTransform.rect.height / Screen.height;
        Vector2 ret = new Vector2(xMultiple * screenPoint.x, yMultiple * screenPoint.y);
        return ret;
    }

    public static void FillItem(Transform child, Tuple<InventoryItemSO, InventoryItem> item)
    {
        if (child.GetChild(1).TryGetComponent<Image>(out Image itemImage))
        {
            itemImage.sprite = item.Item1.sprite;
            itemImage.color = Color.white;
        } else
        {
            Debug.LogError($"J$ Utils: Item Image returned null. See Inventory UI items in Main Canvas");
        }
        string quantity = "";
        if (item.Item2.quantity >= 9999) quantity = "9999";
        else if (item.Item2.quantity != 1) quantity = item.Item2.quantity.ToString();
        if (child.GetChild(2).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI itemText))
        {
            itemText.text = quantity;
        } else
        {
            Debug.LogError($"J$ Utils: Item TMPro returned null. See Inventory UI items in Main Canvas");
        }
    }

    public static void EmptyItem(Transform child)
    {
        if (child.GetChild(1).TryGetComponent<Image>(out Image itemImage))
        {
            itemImage.sprite = null;
            itemImage.color = Color.clear;
        } else
        {
            Debug.LogError($"J$ Utils: Item Image returned null. See Inventory UI items in Main Canvas");
        }
        string quantity = "";
        if (child.GetChild(2).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI itemText))
        {
            itemText.text = quantity;
        } else
        {
            Debug.LogError($"J$ Utils: Item TMPro returned null. See Inventory UI items in Main Canvas");
        }
    }
}
