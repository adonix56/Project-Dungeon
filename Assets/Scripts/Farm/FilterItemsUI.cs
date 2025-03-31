/* 
 * File: FilterItemsUI.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/25/2025 
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A UI representation of the state of the player's Inventory, after filtering by InventoryCategory.
/// </summary>
public class FilterItemsUI : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Event triggered when an item is selected.
    /// </summary>
    public event Action<InventoryItemSO> OnSelectedItem;

    /// <summary>
    /// The parent transform of the inventory
    /// </summary>
    [SerializeField] private Transform itemContainer;
    /// <summary>
    /// A List<> representation of the filtered slots.
    /// </summary>
    [SerializeField] private List<InventoryItemUI> itemSlots = new List<InventoryItemUI>();
    private int filteredItems;
    private FarmUI farmUI;

    private void Start()
    {
        farmUI = GameManager.Instance.GetMainCanvas().GetComponent<FarmUI>();
    }

    /// <summary>
    /// Event Handler when clicking the background.
    /// </summary>
    /// <param name="eventData">The pointer event data</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        ExitFilter();
    }

    /// <summary>
    /// Exits the filter mode and updates the FarmUI.
    /// </summary>
    private void ExitFilter()
    {
        farmUI.ToggleFilter(InventoryCategory.Seeds, false);
    }

    /// <summary>
    /// Populate the empty slots with filtered items, or clear them.
    /// </summary>
    /// <param name="category">The category of items to filter.</param>
    /// <param name="active">True to fill the items, False to clear them.</param>
    public void InitializeFilter(InventoryCategory category, bool active)
    {
        if (active)
        {
            // Filter inventory
            Inventory inventory = GameManager.Instance.GetInventory();
            ItemsWrapper items = inventory.FilterByCategory(category);
            int itemCount = items.Count;

            // Populate each UI slot with filtered items
            for (int i = 0; i < itemCount; i++)
            {
                itemSlots[i].Populate(items.Get(i), false, false);
                itemSlots[i].OnItemSelect += OnItemClicked;
                filteredItems++;
            }
        } else
        {
            // Reset each of the populated UI slots
            for (int i = 0; i < filteredItems; i++)
            {
                Debug.Log("J$ FilterItemsUI Emptying slots");
                itemSlots[i].Empty();
                itemSlots[i].OnItemSelect -= OnItemClicked;
            }
            filteredItems = 0;
        }
    }

    /// <summary>
    /// Event Handler when clicking on a filtered item.
    /// </summary>
    /// <param name="item">The InventoryItem of the clicked filtered slot.</param>
    private void OnItemClicked(InventoryItem item)
    {
        // Deduct from Inventory
        Inventory inventory = GameManager.Instance.GetInventory();
        if (!inventory.TryUseItem(item.inventoryItemSO, 1))
        {
            Debug.LogError($"J$ FilterItemsUI Failed to use {item.inventoryItemSO.itemName}");
        }
        // Replace Empty UI prefab with farm UI prefab
        OnSelectedItem?.Invoke(item.inventoryItemSO);
        ExitFilter();
    }
}
