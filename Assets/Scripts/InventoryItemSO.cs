using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item SO", menuName = "Scriptable Objects/InventoryItemSO")]
public class InventoryItemSO : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    public string description;
    public InventoryCategory category;
    [Tooltip("Usually cost to purchase. For items with Quality, dataA corresponds to the sell price for Quality 0.")]
    public int dataA;
    [Tooltip("For items with Quality, dataB corresponds to the sell price for Quality 100.\n\nFor Seeds, dataB corresponds to the number of seconds until harvest.")]
    public int dataB;
    [Tooltip("For Seeds, the Connected Inventory Item corresponds to the Harvest object that it grows.")]
    public InventoryItemSO connectedInventoryItem;

    public int GetCost(int quality)
    {
        int addition = 0;
        if (HasQuality())
        {
            int difference = dataB - dataA;
            float qualityScale = (float)quality / 100f;
            addition = Mathf.FloorToInt(qualityScale * (float) difference);
        }
        return dataA + addition;
    }

    public bool HasQuality()
    {
        return category == InventoryCategory.Harvest;
    }
}
