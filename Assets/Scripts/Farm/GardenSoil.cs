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

    protected override void Start()
    {
        base.Start();
    }

    public override SelectableSO Select() 
    {
        SelectableSO ret = base.Select();
        foreach (GardenSoilItem item in items)
        {
            FarmItem newItem;
            if (item.IsEmpty()) 
            {
                newItem = farmUI.AddItemToSelectContainer(itemEmptyPrefab);
                newItem.OnClick += EmptyOnClick;
            } else
            {
                newItem = farmUI.AddItemToSelectContainer(itemPrefab);
                newItem.OnClick += GrowingOnClick;
            }
            item.SetFarmItem(newItem);
        }
        return ret;
    }

    private void GrowingOnClick()
    {
        Debug.Log("J$ GardenSoil GrowingOnClick");
    }

    private void EmptyOnClick()
    {
        farmUI.ToggleFilter(InventoryCategory.Seeds, true);
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

    public bool IsEmpty()
    {
        return harvested;
    }

    public bool CanBeHarvested()
    {
        return DateTime.Now > endTime;
    }

    public void SetFarmItem(FarmItem newItem)
    {
        obj = newItem;
    }
}

