/* 
 * File: Inventory.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/25/2025 
 */

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Manages the state of the player's inventory, including adding, filtering and using items.
/// </summary>
public class Inventory : MonoBehaviour
{
    /// <summary>
    /// Event triggered when gold amount changed.
    /// </summary>
    public event Action<int> OnGoldChange;
    /// <summary>
    /// The initial inventory items and their quantities.
    /// </summary>
    [SerializeField]
    private InventoryDictionaryWrapper startingInventory;

    /// <summary>
    /// A dictionary that holds the inventory items and their details.
    /// </summary>
    private Dictionary<InventoryItemSO, List<InventoryItem>> inventoryItems;

    [SerializeField]
    private InventoryItemSO goldSO;

    private void Awake()
    {
        BuildDictionary();
    }

    /// <summary>
    /// Builds the initial state of the inventory.
    /// </summary>
    private void BuildDictionary()
    {
        inventoryItems = new Dictionary<InventoryItemSO, List<InventoryItem>>();
        foreach (InventoryKeyValuePair item in startingInventory.keyValuePairs)
        {
            inventoryItems.Add(item.key, item.value);
        }
    }

    /// <summary>
    /// Performs an action to each individual item.
    /// </summary>
    /// <param name="action">The action to perform on each item.</param>
    /// <returns>A wrapper containing a list of items created based on the action provided.</returns>
    private ItemsWrapper ForeachInventoryItem(Action<ItemsWrapper, InventoryItemSO, InventoryItem> action)
    {
        ItemsWrapper items = new ItemsWrapper();
        foreach (InventoryItemSO itemSO in inventoryItems.Keys)
        {
            foreach (InventoryItem item in inventoryItems[itemSO])
            {
                action(items, itemSO, item);
            }
        }
        return items;
    }

    /// <summary>
    /// Gets all inventory items.
    /// </summary>
    /// <returns>A wrapper containing all inventory items.</returns>
    public ItemsWrapper GetAllInventoryItems() 
    {
        return ForeachInventoryItem((items, itemSO, item) => items.items.Add(new Tuple<InventoryItemSO, InventoryItem>(itemSO, item)));
    }

    /// <summary>
    /// Gets a list of inventory items after filtering by category.
    /// </summary>
    /// <param name="category">Category to filter by.</param>
    /// <returns>A wrapper containing all filtered items.</returns>
    public ItemsWrapper FilterByCategory(InventoryCategory category)
    {
        Action<ItemsWrapper, InventoryItemSO, InventoryItem> action = (items, itemSO, item) =>
        {
            if (itemSO.category == category)
            {
                items.items.Add(new Tuple<InventoryItemSO, InventoryItem>(itemSO, item));
            }
        };

        return ForeachInventoryItem(action);
    }

    /// <summary>
    /// Adds an item to inventory.
    /// </summary>
    /// <param name="inventoryItemSO">InventoryItemSO of the item to add.</param>
    /// <param name="inventoryItem">InventoryItemDetails of the item to add.</param>
    public void AddItem(InventoryItemSO inventoryItemSO, InventoryItem inventoryItem)
    {
        if (inventoryItems.ContainsKey(inventoryItemSO))
        {
            List<InventoryItem> itemList = inventoryItems[inventoryItemSO];
            int index = 0;
            if (inventoryItemSO.HasQuality())
            {
                index = itemList.FindIndex(item => item.quality == inventoryItem.quality);
            }
            // Inventory has item, but not matching quality.
            if (index < 0)
            {
                itemList.Add(inventoryItem);
                return;
            }
            // Inventory has the item, increase the quantity.
            int newQuantity = inventoryItem.quantity + itemList[index].quantity;
            itemList[index] = new InventoryItem(inventoryItem.quality, newQuantity);
        } else
        {
            inventoryItems.Add(inventoryItemSO, new List<InventoryItem> { inventoryItem });
        }
    }

    /// <summary>
    /// Attempts to use the specified inventory item.
    /// </summary>
    /// <param name="useItem">The InventoryItemSO to attempt to use.</param>
    /// <param name="useQuantity">The quantity of the item to use. Default is 1.</param>
    /// <param name="quality">The quality of the item to use. Default is 0.</param>
    /// <returns>True if the item was successfully used; otherwise, false.</returns>
    public bool TryUseItem(InventoryItemSO useItem, int useQuantity = 1, int quality = 0)
    {
        List<InventoryItem> itemList = inventoryItems[useItem];
        int index = 0;
        InventoryItem item = itemList[index];
        // If the item has a quality, try to find an existing item with matching quality.
        if (useItem.HasQuality())
        {
            bool found = false;
            for (int i = 1; i < itemList.Count; i++) { 
                if (itemList[i].quality == quality)
                {
                    found = true;
                    item = itemList[i];
                    break;
                }
            }
            if (!found)
            {
                Debug.Log($"J$ Inventory Trying to use {useItem.itemName} with a quality of {quality}, but the player does not own any.");
                return false;
            }
        }
        if (item.quantity <= 0)
        {
            Debug.LogError($"J$ Inventory Trying to use {useItem.itemName}, but player does not own any.");
            return false;
        }
        item.quantity -= useQuantity;
        if (item.quantity <= 0)
        {
            if (itemList.Count == 1)
            {
                inventoryItems.Remove(useItem);
            } else
            {
                itemList.RemoveAt(index);
            }
        }
        itemList[index] = item;
        return true;
    }

    /// <summary>
    /// Retrieves the current gold amount.
    /// </summary>
    /// <returns>The current gold amount.</returns>
    public int GetGold()
    {
        return inventoryItems[goldSO][0].quantity;
    }

    /// <summary>
    /// Changes the gold amount by the amount given.
    /// </summary>
    /// <param name="amount">The amount of gold to add or remove.</param>
    public void ChangeGold(int amount)
    {
        if (amount > 0)
        {
            AddItem(goldSO, new InventoryItem(0, amount));
        } else
        {
            TryUseItem(goldSO, amount * -1);
        }
        OnGoldChange?.Invoke(GetGold());
    }

    public void TESTING()
    {
        StringBuilder stringBuilder = new StringBuilder();
        ForeachInventoryItem((items, itemSO, item) => stringBuilder.Append($"{itemSO.itemName}\t{item.quality}\tx{item.quantity}\n"));
        Debug.Log(stringBuilder.ToString());
    }
}
