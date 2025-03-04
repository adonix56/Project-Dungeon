/* 
 * File: FarmUI.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/19/2025 
 */

using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the main UI for the farming game.
/// </summary>
public class FarmUI : MonoBehaviour
{
    private const string DEACTIVATE = "Deactivate";

    [Header("MainUI")]
    [SerializeField] private MainUI mainUI;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("SelectUI")]
    [SerializeField] private Animator selectUI;
    [SerializeField] private TextMeshProUGUI selectTitle;
    [SerializeField] private Transform selectItemContainer;

    [Header("InventoryUI")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("FilterUI")]
    [SerializeField] private FilterItemsUI filterUI;

    [Header("ShopUI")]
    [SerializeField] private ShopUI shopUI;
    /// <summary>
    /// Event triggered when a filtered item is selected.
    /// </summary>
    public event Action<InventoryItemSO> OnFilterSelected;

    /// <summary>
    /// Enumeration of different UI Segments
    /// </summary>
    public enum UISegment {
        None, MainUI, SelectUI, InventoryUI, FilterUI, ShopUI
    }

    /// <summary>
    /// Activates or deactivates the different UISegments.
    /// </summary>
    /// <param name="active">Whether to activate or deactivate the segment.</param>
    /// <param name="uiSegment">Which segment to activate</param>
    /// <param name="obj">Selected object to pass information to UI. Default is null.</param>
    public void SetUIObjectActive(bool active, UISegment uiSegment, SelectableSO obj = null)
    {
        if (uiSegment == UISegment.SelectUI)
        {
            if (active)
            {
                selectTitle.text = obj.title;
                selectUI.gameObject.SetActive(true);
                mainUI.gameObject.SetActive(false);
            }
            if (!active)
            {
                selectUI.SetTrigger(DEACTIVATE);
                mainUI.gameObject.SetActive(true);
            }
        } else if (uiSegment == UISegment.InventoryUI)
        {
            inventoryUI.gameObject.SetActive(active);
        } else if (uiSegment == UISegment.FilterUI)
        {
            if (!active) filterUI.OnSelectedItem -= FilterUI_OnSelectedItem;
            filterUI.gameObject.SetActive(active);
            if (active) filterUI.OnSelectedItem += FilterUI_OnSelectedItem;
        } else if (uiSegment == UISegment.ShopUI)
        {
            mainUI.gameObject.SetActive(!active);
            shopUI.gameObject.SetActive(active);
        }

    }

    /// <summary>
    /// Invokes event when a filtered item is selected.
    /// </summary>
    /// <param name="selectedFilterItem">The selected filtered item.</param>
    private void FilterUI_OnSelectedItem(InventoryItemSO selectedFilterItem)
    {
        OnFilterSelected?.Invoke(selectedFilterItem);
    }

    /// <summary>
    /// Toggles the Inventory UI.
    /// </summary>
    /// <param name="active">Whether to activate or deactivate the inventory.</param>
    public void ToggleInventory(bool active)
    {
        inventoryUI.InitializeInventoryUI(active);
        SetUIObjectActive(active, UISegment.InventoryUI);
    }

    /// <summary>
    /// Toggles the Filter UI.
    /// </summary>
    /// <param name="category">Which category to filter by.</param>
    /// <param name="active">Whether to activate or deactivate the inventory.</param>
    public void ToggleFilter(InventoryCategory category, bool active)
    {
        filterUI.InitializeFilter(category, active);
        SetUIObjectActive(active, UISegment.FilterUI);
    }

    /// <summary>
    /// Adds a FarmItemUI object to the SelectUI container.
    /// </summary>
    /// <param name="itemObject">Prefab to instantiate</param>
    /// <returns>The FarmItemUI component from the newly instantiated GameObject.</returns>
    public FarmItemUI AddItemToSelectContainer(GameObject itemObject)
    {
        GameObject newObject = Instantiate(itemObject);
        newObject.transform.SetParent(selectItemContainer, false);
        return newObject.GetComponent<FarmItemUI>();
    }

    /// <summary>
    /// Adds a UI Element to the Farm UI
    /// </summary>
    /// <param name="itemObject">Prefab to instantiate.</param>
    /// <returns>A reference to the newly instantiated GameObject.</returns>
    public GameObject AddItemToFarmUI(GameObject itemObject)
    {
        GameObject newObject = Instantiate(itemObject);
        newObject.transform.SetParent(transform, false);
        return newObject;
    }

    /// <summary>
    /// Clears the SelectUI container of FarmItemUI objects.
    /// </summary>
    public void EmptySelectContainer()
    {
        int childCount = selectItemContainer.childCount; // Must save value since Destroying changes it.
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(selectItemContainer.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Sets the UI to display the gold amount.
    /// </summary>
    /// <param name="newGoldValue"></param>
    public void SetGold(int newGoldValue)
    {
        goldText.text = newGoldValue.ToString();
    }
}
