using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FilterItemsUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform itemContainer;
    [SerializeField] private List<InventoryItemSO> filteredItems;
    private FarmUI farmUI;

    private void Start()
    {
        farmUI = GameManager.Instance.GetMainCanvas().GetComponent<FarmUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ExitFilter();
    }

    private void ExitFilter()
    {
        farmUI.ToggleFilter(InventoryCategory.Seeds, false);
    }

    public void OnItemClicked(int index)
    {
        if (index < filteredItems.Count) {
            Debug.Log("J$ FilterItemsUI clicked object");
            // Deduct from Inventory
            Inventory inventory = GameManager.Instance.GetInventory();
            if (!inventory.TryUseItem(filteredItems[index])) {
                Debug.LogError($"J$ FilterItemsUI Failed to use {filteredItems[index].itemName}");
            }
            // Replace Empty UI prefab with farm UI prefab
            ExitFilter();
        }
    }

    public void InitializeFilter(InventoryCategory category, bool active)
    {
        Inventory inventory = GameManager.Instance.GetInventory();
        ItemsWrapper items = inventory.FilterByCategory(category);
        int itemCount = items.Count;
        if (active)
        {
            for (int i = 0; i < itemCount; i++)
            {
                Transform child = itemContainer.GetChild(i);
                AddItem(items.Get(i).Item1);
                Utils.FillItem(child, items.Get(i));
            }
        } else
        {
            for (int i = 0; i < filteredItems.Count; i++)
            {
                Utils.EmptyItem(itemContainer.GetChild(i));
            }
            ClearItems();
        }
    }

    public void AddItem(InventoryItemSO item) 
    { 
        filteredItems.Add(item);
    }

    public void ClearItems()
    {
        filteredItems.Clear();
    }
}
