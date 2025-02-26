using System;
using System.Collections.Generic;
using UnityEngine;

public class GardenSoil : BasePlacement
{
    [Header("GardenSoil")]
    [SerializeField]
    private List<GardenSoilItem> items;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private GameObject itemEmptyPrefab;
    private Action<InventoryItemSO> currentFilterAction;

    protected override void Start()
    {
        base.Start();
    }

    public override SelectableSO Select() 
    {
        SelectableSO ret = base.Select();
        for (int i = 0; i < items.Count; i++)
        {
            FarmItem newItem;
            if (items[i].IsEmpty()) 
            {
                newItem = farmUI.AddItemToSelectContainer(itemEmptyPrefab);
                newItem.OnClick += EmptyOnClick;
            } else
            {
                newItem = farmUI.AddItemToSelectContainer(itemPrefab);
                newItem.OnClick += GrowingOnClick;
            }
            items[i] = new GardenSoilItem(items[i], newItem);
        }
        return ret;
    }

    private void GrowingOnClick(FarmItem clickedItem)
    {
        int currentSiblingIndex = clickedItem.transform.GetSiblingIndex();
        if (items[currentSiblingIndex].CanBeHarvested())
        {
            Debug.Log($"J$ GardenSoil: Can harvest!");
        } else
            Debug.Log($"J$ GardenSoil: Cannot harvest!");
    }

    private void EmptyOnClick(FarmItem clickedItem)
    {
        farmUI.ToggleFilter(InventoryCategory.Seeds, true);
        currentFilterAction = (filtered) => FarmUI_OnFilterSelected(filtered, clickedItem);
        farmUI.OnFilterSelected += currentFilterAction;
    }

    private void FarmUI_OnFilterSelected(InventoryItemSO obj, FarmItem clickedItem)
    {
        farmUI.OnFilterSelected -= currentFilterAction;
        currentFilterAction = null;

        // Replace FarmItem from Empty to Active
        int currentSiblingIndex = clickedItem.transform.GetSiblingIndex();
        Destroy(items[currentSiblingIndex].obj.gameObject);
        FarmItem newItem = farmUI.AddItemToSelectContainer(itemPrefab);
        newItem.OnClick += GrowingOnClick;
        newItem.transform.SetSiblingIndex(currentSiblingIndex);

        // Point the new items[currentSiblingIndex] to the new struct
        GardenSoilItem newGardenSoilItem = new GardenSoilItem(obj.itemName, DateTime.Now, DateTime.Now.AddSeconds(obj.dataB)); //dataB corresponds to seconds until harvest
        newItem.SetValues(newGardenSoilItem.name, obj.description, newGardenSoilItem.GetTimeLeft(), "0", obj.sprite, newGardenSoilItem.GetPercentageLeft());
        items[currentSiblingIndex] = newGardenSoilItem;
    }
}

[Serializable]
public struct GardenSoilItem
{
    public string name;
    public DateTime startTime;
    public DateTime endTime;
    public bool harvested;
    public FarmItem obj;

    public GardenSoilItem(string n, DateTime st, DateTime et)
    {
        name = n;
        startTime = st;
        endTime = et;
        harvested = false;
        obj = null;
    }

    public GardenSoilItem(GardenSoilItem copy, FarmItem newFarmObject)
    {
        name = copy.name;
        startTime = copy.startTime;
        endTime = copy.endTime;
        harvested = copy.harvested;
        obj = newFarmObject;
    }

    public bool IsEmpty()
    {
        return harvested;
    }

    public bool CanBeHarvested()
    {
        return DateTime.Now > endTime;
    }

    public string GetTimeLeft()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeDifference = endTime - currentTime;
        int totalHours = (int)timeDifference.TotalHours;
        int totalMinutes = timeDifference.Minutes;
        int totalSeconds = timeDifference.Seconds;

        return string.Format("{0:D2}:{1:D2}:{2:D2}", totalHours, totalMinutes, totalSeconds);
    }

    public float GetPercentageLeft()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan totalDifference = endTime - startTime;
        TimeSpan currentDifference = endTime - currentTime;

        return (float)((totalDifference.TotalSeconds - currentDifference.TotalSeconds) / totalDifference.TotalSeconds);
    }
}

