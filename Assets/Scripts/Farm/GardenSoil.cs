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
            items[i].farmItem = newItem;// = new GardenSoilItem(items[i], newItem);
        }
        return ret;
    }

    private void GrowingOnClick(FarmItem clickedItem)
    {
        int currentSiblingIndex = clickedItem.transform.GetSiblingIndex();
        if (items[currentSiblingIndex].CanBeHarvested())
        {
            Destroy(items[currentSiblingIndex].farmItem.gameObject);
            FarmItem newItem = farmUI.AddItemToSelectContainer(itemEmptyPrefab);
            newItem.OnClick += EmptyOnClick;
            newItem.transform.SetSiblingIndex(currentSiblingIndex);
            items[currentSiblingIndex].Harvest(newItem);
        }
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
        Destroy(items[currentSiblingIndex].farmItem.gameObject);
        FarmItem newItem = farmUI.AddItemToSelectContainer(itemPrefab);
        newItem.OnClick += GrowingOnClick;
        newItem.transform.SetSiblingIndex(currentSiblingIndex);

        // Point the new items[currentSiblingIndex] to the new struct
        //GardenSoilItem newGardenSoilItem = new GardenSoilItem(newItem, obj);
        items[currentSiblingIndex].PlantSeeds(obj);
        items[currentSiblingIndex].farmItem = newItem;
        //newItem.SetValues(newGardenSoilItem, obj.description, newGardenSoilItem.GetTimeLeft(), "0", obj.sprite, newGardenSoilItem.GetPercentageLeft());
        newItem.PlantSeeds(items[currentSiblingIndex]);
        //items[currentSiblingIndex] = newGardenSoilItem;
    }
}

