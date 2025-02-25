using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemContainer;
    [SerializeField] private ItemsWrapper items;

    public void InitializeInventoryUI(bool active)
    {
        Inventory inventory = GameManager.Instance.GetInventory();
        ItemsWrapper items = inventory.GetAllInventoryItems();
        int itemCount = items.Count;
        if (active)
        {
            for (int i = 0; i < itemCount; i++)
            {
                Transform child = itemContainer.GetChild(i);
                Utils.FillItem(child, items.Get(i));
            }
        } else
        {
            //TODO: Empty Items
        }
    }
}
