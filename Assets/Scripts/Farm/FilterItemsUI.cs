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
    /// A List<> representation of the filtered items.
    /// </summary>
    [SerializeField] private List<InventoryItemSO> filteredItems;
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
    /// Event Handler when clicking on a filtered item.
    /// </summary>
    /// <param name="index">The index of the clicked item in the filtered list</param>
    public void OnItemClicked(int index)
    {
        if (index < filteredItems.Count) {
            // Deduct from Inventory
            Inventory inventory = GameManager.Instance.GetInventory();
            if (!inventory.TryUseItem(filteredItems[index])) {
                Debug.LogError($"J$ FilterItemsUI Failed to use {filteredItems[index].itemName}");
            }
            // Replace Empty UI prefab with farm UI prefab
            OnSelectedItem?.Invoke(filteredItems[index]);
            ExitFilter();
        }
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
                Transform child = itemContainer.GetChild(i);
                filteredItems.Add(items.Get(i).Item1);
                Utils.FillItem(child, items.Get(i));
            }
        } else
        {
            // Reset each of the populated UI slots
            for (int i = 0; i < filteredItems.Count; i++)
            {
                Utils.EmptyItem(itemContainer.GetChild(i));
            }
            filteredItems.Clear();
        }
    }
}
