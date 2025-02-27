/* 
 * File: InventoryItemSO.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/25/2025 
 */

using UnityEngine;

/// <summary>
/// Represents a Scriptable Object for inventory items, including properties and behaviors.
/// </summary>
[CreateAssetMenu(fileName = "New Inventory Item SO", menuName = "Scriptable Objects/InventoryItemSO")]
public class InventoryItemSO : ScriptableObject
{
    /// <summary>
    /// The name of the inventory item.
    /// </summary>
    public string itemName;
    /// <summary>
    /// The sprite of the inventory item.
    /// </summary>
    public Sprite sprite;
    /// <summary>
    /// The description of the inventory item.
    /// </summary>
    public string description;
    /// <summary>
    /// The category of the inventory item.
    /// </summary>
    public InventoryCategory category;
    /// <summary>
    /// Usually the cost to purchase. For items with Quality, dataA corresponds to the sell price for Quality 0.
    /// </summary>
    [Tooltip("Usually the cost to purchase or sale. For items with Quality, dataA corresponds to the sell price for Quality 0.")]
    public int dataA;
    /// <summary>
    /// For items with Quality, dataB corresponds to the sell price for Quality 100.
    /// For Seeds, dataB corresponds to the number of seconds until harvest.
    /// </summary>
    [Tooltip("For items with Quality, dataB corresponds to the sell price for Quality 100.\n\nFor Seeds, dataB corresponds to the number of seconds until harvest.")]
    public int dataB;
    /// <summary>
    /// For Seeds, the Connected Inventory Item corresponds to the Harvest object that it grows.
    /// </summary>
    [Tooltip("For Seeds, the Connected Inventory Item corresponds to the Harvest object that it grows.")]
    public InventoryItemSO connectedInventoryItem;

    /// <summary>
    /// Calculates the cost of the item based on its quality.
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public int GetCost(int quality)
    {
        int addition = 0;
        if (HasQuality())
        {
            // Use a linear scale where 0 corresponds to lowest price, and 100 to highest price.
            int difference = dataB - dataA;
            float qualityScale = (float)quality / 100f;
            addition = Mathf.FloorToInt(qualityScale * (float) difference);
        }
        return dataA + addition;
    }

    /// <summary>
    /// Determines whether the item has quality attributes.
    /// </summary>
    /// <returns>True if the item has quality attributes; otherwise, false.</returns>
    public bool HasQuality()
    {
        return category == InventoryCategory.Harvest;
    }
}
