using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private InventoryDictionaryWrapper startingInventory;

    private Dictionary<InventoryItemSO, List<InventoryItem>> inventoryItems;

    private void Awake()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        inventoryItems = new Dictionary<InventoryItemSO, List<InventoryItem>>();
        foreach (InventoryKeyValuePair item in startingInventory.keyValuePairs)
        {
            inventoryItems.Add(item.key, item.value);
        }
    }

    private ItemsWrapper FilterInventoryItems(Action<ItemsWrapper, InventoryItemSO, InventoryItem> action)
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

    public ItemsWrapper GetAllInventoryItems() 
    {
        return FilterInventoryItems((items, itemSO, item) => items.items.Add(new Tuple<InventoryItemSO, InventoryItem>(itemSO, item)));
    }

    public ItemsWrapper FilterByCategory(InventoryCategory category)
    {
        Action<ItemsWrapper, InventoryItemSO, InventoryItem> action = (items, itemSO, item) =>
        {
            if (itemSO.category == category)
            {
                items.items.Add(new Tuple<InventoryItemSO, InventoryItem>(itemSO, item));
            }
        };

        return FilterInventoryItems(action);
    }

    public bool TryUseItem(InventoryItemSO useItem, int useQuantity = 1, int quality = 0)
    {
        List<InventoryItem> itemList = inventoryItems[useItem];
        int index = 0;
        InventoryItem item = itemList[index];
        if (useItem.hasQuality)
        {
            //TODO: Handle Quality usage
        }
        if (item.quantity <= 0)
        {
            Debug.LogError($"J$ Inventory Trying to use {useItem.itemName}, but player does not own any.");
            return false;
        }
        item.quantity--;
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

    public void TESTING()
    {
        StringBuilder stringBuilder = new StringBuilder();
        FilterInventoryItems((items, itemSO, item) => stringBuilder.Append($"{itemSO.itemName}\t{item.quality}\tx{item.quantity}\n"));
        Debug.Log(stringBuilder.ToString());
    }
}
