/* 
 * File: GardenSoil.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/19/2025 
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Controls the state of Garden Soil Farm objects.
/// </summary>
public class GardenSoil : BasePlacement
{
    [Header("GardenSoil")]
    [SerializeField] private List<GardenSoilItem> items;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject itemEmptyPrefab;
    [SerializeField] private GameObject harvestResultPrefab;
    private Action<InventoryItemSO> currentFilterAction;

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// When selected, fill the UI container with empty or active plants.
    /// </summary>
    /// <returns>The current SelectableSO for this building</returns>
    public override SelectableSO Select() 
    {
        SelectableSO ret = base.Select();
        for (int i = 0; i < items.Count; i++)
        {
            FarmItemUI newItem;
            if (items[i].IsEmpty())
            {
                // Add Empty Prefab if nothing is growing
                newItem = farmUI.AddItemToSelectContainer(itemEmptyPrefab);
                newItem.OnClick += EmptyOnClick;
            } else
            {
                // Add Active Prefab if something is growing and setup timer
                newItem = farmUI.AddItemToSelectContainer(itemPrefab);
                newItem.OnClick += GrowingOnClick;
                newItem.PlantSeeds(items[i]);
            }
            // Save reference to UI object
            items[i].farmItem = newItem;
        }
        return ret;
    }

    /// <summary>
    /// Subscriber for FarmItem OnClick Events that have seeds already planted.
    /// </summary>
    /// <param name="clickedItem">Reference to the FarmItem that was clicked.</param>
    private void GrowingOnClick(FarmItemUI clickedItem)
    {
        int currentSiblingIndex = clickedItem.transform.GetSiblingIndex();
        if (items[currentSiblingIndex].CanBeHarvested())
        {
            GardenSoilItem harvestItem = items[currentSiblingIndex];
            // Harvest FarmItem from Active to Empty
            harvestItem.Harvest(ReplaceFarmItem(currentSiblingIndex, false));
            // Check if crit
            int quantity = 3;
            if (GameManager.Instance.CheckGardenCrit()) quantity *= 2;
            // Check quality
            int quality = Random.Range(0, 101);
            GameManager.Instance.GetInventory().AddItem(harvestItem.seedItemSO.connectedInventoryItem, new InventoryItem(quality, quantity));
            HarvestResult harvestResult = farmUI.AddItemToFarmUI(harvestResultPrefab).GetComponent<HarvestResult>();
            harvestResult.SetupResult(harvestItem.seedItemSO.connectedInventoryItem, quality, quantity == 6);
            Debug.Log($"J$ GardenSoil Harvested {quantity} {harvestItem.seedItemSO.connectedInventoryItem.itemName}s of Quality {quality}.");
        } else
        {
            Debug.Log("J$ GardenSoil Cannot be harvested");
        }
    }

    /// <summary>
    /// Subscriber for FarmItem OnClick Events that have nothing planted.
    /// </summary>
    /// <param name="clickedItem">Reference to the FarmItem that was clicked.</param>
    private void EmptyOnClick(FarmItemUI clickedItem)
    {
        // Open Filter to show Seeds
        farmUI.ToggleFilter(InventoryCategory.Seeds, true);
        currentFilterAction = (filtered) => FarmUI_OnFilterSelected(filtered, clickedItem);
        farmUI.OnFilterSelected += currentFilterAction;
    }

    /// <summary>
    /// Subscriber to the event when a filtered item was selected.
    /// </summary>
    /// <param name="obj">The filtered item selected.</param>
    /// <param name="clickedItem">Reference to the FarmItem that was clicked.</param>
    private void FarmUI_OnFilterSelected(InventoryItemSO obj, FarmItemUI clickedItem)
    {
        // Reset Filter Action
        farmUI.OnFilterSelected -= currentFilterAction;
        currentFilterAction = null;

        // Replace FarmItem from Empty to Active
        int currentSiblingIndex = clickedItem.transform.GetSiblingIndex();
        items[currentSiblingIndex].farmItem = ReplaceFarmItem(currentSiblingIndex, true);

        // Plant Seeds
        items[currentSiblingIndex].PlantSeeds(obj);
        items[currentSiblingIndex].farmItem.PlantSeeds(items[currentSiblingIndex]);
    }

    /// <summary>
    /// Replaces the current FarmItem displayed on the UI with the opposite prefab. 
    /// i.e. Empty prefabs replaced with Active prefabs and vice versa.
    /// </summary>
    /// <param name="index">The index of targeted FarmItem clicked.</param>
    /// <param name="removeEmpty">True to replace Empty with Active, False to replace Active with Empty.</param>
    /// <returns>The new FarmItem created.</returns>
    private FarmItemUI ReplaceFarmItem(int index, bool removeEmpty)
    {
        // Remove Subscriber
        if (removeEmpty) items[index].farmItem.OnClick -= EmptyOnClick;
        else items[index].farmItem.OnClick -= GrowingOnClick;

        // Destroy FarmItem
        Destroy(items[index].farmItem.gameObject);

        // Create replacement FarmItem
        FarmItemUI newItem = removeEmpty ? farmUI.AddItemToSelectContainer(itemPrefab) : farmUI.AddItemToSelectContainer(itemEmptyPrefab);

        // Resubscribe
        if (removeEmpty) newItem.OnClick += GrowingOnClick;
        else newItem.OnClick += EmptyOnClick;

        // Place in the same location
        newItem.transform.SetSiblingIndex(index);
        return newItem;
    }
}

