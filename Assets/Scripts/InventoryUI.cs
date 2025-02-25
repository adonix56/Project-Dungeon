using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemContainer;
    [SerializeField] private ItemsWrapper inventoryItems;

    public void InitializeInventoryUI(bool active)
    {
        Inventory inventory = GameManager.Instance.GetInventory();
        if (active)
        {
            inventoryItems = inventory.GetAllInventoryItems();
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                Transform child = itemContainer.GetChild(i);
                Utils.FillItem(child, inventoryItems.Get(i));
            }
        } else
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                Utils.EmptyItem(itemContainer.GetChild(i));
            }
            inventoryItems.Clear();
        }
    }
}
