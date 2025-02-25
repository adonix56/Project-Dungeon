using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item SO", menuName = "Scriptable Objects/InventoryItemSO")]
public class InventoryItemSO : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    public string description;
    public InventoryCategory category;
    public bool hasQuality;
    public int lowValue;
    public int highValue;

    public int GetCost(int quality)
    {
        int addition = 0;
        if (hasQuality)
        {
            int difference = highValue - lowValue;
            float qualityScale = (float)quality / 100f;
            addition = Mathf.FloorToInt(qualityScale * (float) difference);
        }
        return lowValue + addition;
    }
}
