/* 
 * File: Utils.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/25/2025 
 */

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents an item in the inventory
/// </summary>
[Serializable]
public struct InventoryItem
{
    /// <summary>
    /// The quality of the inventory item.
    /// </summary>
    public int quality;
    /// <summary>
    /// The quantity of the inventory item.
    /// </summary>
    public int quantity;

    /// <summary>
    /// Constructor to create a new Inventory Item entry.
    /// </summary>
    /// <param name="quality">Quality of the item.</param>
    /// <param name="quantity">Quantity of the item.</param>
    public InventoryItem (int quality, int quantity)
    {
        this.quality = quality;
        this.quantity = quantity;
    }
}

/// <summary>
/// Enumeration of inventory item categories.
/// </summary>
public enum InventoryCategory
{
    Gold, Harvest, Seeds
}

/// <summary>
/// Represents a key-value pair for inventory items and their details
/// </summary>
[Serializable]
public class InventoryKeyValuePair
{
    /// <summary>
    /// The InventoryItemSO representing the key.
    /// </summary>
    public InventoryItemSO key;
    /// <summary>
    /// A list of items with the same InventoryItemSO key.
    /// </summary>
    public List<InventoryItem> value; 
}

/// <summary>
/// A wrapper for a list of inventory key-value pairs.
/// </summary>
[Serializable]
public class InventoryDictionaryWrapper
{
    /// <summary>
    /// The list of inventory key-value pairs.
    /// </summary>
    public List<InventoryKeyValuePair> keyValuePairs = new List<InventoryKeyValuePair>();
}

/// <summary>
/// A wrapper class for a list version of inventory items.
/// </summary>
[Serializable]
public class ItemsWrapper
{
    /// <summary>
    /// The list of tuples representing each InventoryItemSO and a list of instances of said SO.
    /// </summary>
    public List<Tuple<InventoryItemSO, InventoryItem>> items = new List<Tuple<InventoryItemSO, InventoryItem>>();
    /// <summary>
    /// Gets the number of items.
    /// </summary>
    public int Count { get { return items.Count; } }
    /// <summary>
    /// Gets an item at a specified index.
    /// </summary>
    /// <param name="index">The index of the item to retrieve.</param>
    /// <returns>The inventory item tuple at the specified index.</returns>
    public Tuple<InventoryItemSO, InventoryItem> Get(int index) {  return items[index]; }
    /// <summary>
    /// Creates and Adds a inventory item Tuple to the list.
    /// </summary>
    /// <param name="itemSO">The InventoryItemSO to add.</param>
    /// <param name="item">The item details to add.</param>
    public void Add(InventoryItemSO itemSO, InventoryItem item) { items.Add(new Tuple<InventoryItemSO, InventoryItem>(itemSO, item)); }
    /// <summary>
    /// Clears the entire inventory List.
    /// </summary>
    public void Clear() { items.Clear(); }
}

/// <summary>
/// Static utility methods for various game-related tasks.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Converts a screen point to a position on the UI Canvas.
    /// </summary>
    /// <param name="screenPoint">The screen point to convert.</param>
    /// <param name="canvas">The canvas to which the point will be converted to.</param>
    /// <returns>The converted position on the canvas.</returns>
    public static Vector2 GetCanvasPositionFromScreenPoint(Vector2 screenPoint, Canvas canvas)
    {
        // Canvas is scaled with Screen Size to match 1920x1080 by width.
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        // Calculate the multiples for each dimension.
        float xMultiple = rectTransform.rect.width / Screen.width;
        float yMultiple = rectTransform.rect.height / Screen.height;
        // Convert the points.
        Vector2 ret = new Vector2(xMultiple * screenPoint.x, yMultiple * screenPoint.y);
        return ret;
    }

    /// <summary>
    /// Fills the UI element with the given inventory item details.
    /// </summary>
    /// <param name="baseTransform">The UI element to fill.</param>
    /// <param name="item">The inventory item details to fill with.</param>
    public static void FillItem(Transform baseTransform, Tuple<InventoryItemSO, InventoryItem> item)
    {
        // NOTE: This works with a heirarchy of baseTransform (parent) -> Image-Image-TextMeshPro (siblings)
        if (baseTransform.GetChild(1).TryGetComponent<Image>(out Image itemImage))
        {
            itemImage.sprite = item.Item1.sprite;
            itemImage.color = Color.white;
        } else
        {
            Debug.LogError($"J$ Utils: Item Image returned null. See Inventory UI items in Main Canvas");
        }
        string quantity = "";
        // Maximize display number to 9999, or don't display a number if only 1 exists.
        if (item.Item2.quantity >= 9999) quantity = "9999";
        else if (item.Item2.quantity != 1) quantity = item.Item2.quantity.ToString();
        if (baseTransform.GetChild(2).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI itemText))
        {
            itemText.text = quantity;
        } else
        {
            Debug.LogError($"J$ Utils: Item TMPro returned null. See Inventory UI items in Main Canvas");
        }
    }

    /// <summary>
    /// Empties the UI element.
    /// </summary>
    /// <param name="baseTransform">The UI element to empty</param>
    public static void EmptyItem(Transform baseTransform)
    {
        // NOTE: This works with a heirarchy of baseTransform (parent) -> Image-Image-TextMeshPro (siblings)
        if (baseTransform.GetChild(1).TryGetComponent<Image>(out Image itemImage))
        {
            itemImage.sprite = null;
            itemImage.color = Color.clear;
        } else
        {
            Debug.LogError($"J$ Utils: Item Image returned null. See Inventory UI items in Main Canvas");
        }
        string quantity = "";
        if (baseTransform.GetChild(2).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI itemText))
        {
            itemText.text = quantity;
        } else
        {
            Debug.LogError($"J$ Utils: Item TMPro returned null. See Inventory UI items in Main Canvas");
        }
    }
}
