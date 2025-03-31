/* 
 * File: InventoryUI.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/25/2025 
 */

using UnityEngine;

/// <summary>
/// A UI representation of the state of the player's Inventory.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// The parent transform of the entire inventory.
    /// </summary>
    [SerializeField] 
    private Transform itemContainer;
    /// <summary>
    /// A List<> representation of the inventory.
    /// </summary>
    [SerializeField] 
    private ItemsWrapper inventoryItems;

    /// <summary>
    /// Populate the empty slots with items, or clear them.
    /// </summary>
    /// <param name="active">True to fill the items, False to clear them.</param>
    public void InitializeInventoryUI(bool active)
    {
        Inventory inventory = GameManager.Instance.GetInventory();
        if (active)
        {
            // Get entire Inventory and populate the UI slots
            inventoryItems = inventory.GetAllInventoryItems();
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                Transform child = itemContainer.GetChild(i);
                //Utils.FillItem(child, inventoryItems.Get(i));
            }
        } else
        {
            // Reset each of the populated UI slots
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                Utils.EmptyItem(itemContainer.GetChild(i));
            }
            inventoryItems.Clear();
        }
    }
}
