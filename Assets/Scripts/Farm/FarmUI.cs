using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmUI : MonoBehaviour
{
    private const string DEACTIVATE = "Deactivate";

    [Header("MainUI")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("SelectUI")]
    [SerializeField] private Animator selectUI;
    [SerializeField] private TextMeshProUGUI selectTitle;
    [SerializeField] private Transform itemContainer;

    [Header("InventoryUI")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("FilterUI")]
    [SerializeField] private FilterItemsUI filterUI;

    public enum UIObject {
        None, MainUI, SelectUI, InventoryUI, FilterUI
    }

    public void SetUIObjectActive(bool active, UIObject uiObject, SelectableSO obj = null)
    {
        //Debug.Log($"J$ FarmUI {obj.UIObject.ToString()} {active}");
        if (uiObject == UIObject.SelectUI)
        {
            if (active)
            {
                selectTitle.text = obj.UITitle;
                selectUI.gameObject.SetActive(true);
                mainUI.SetActive(false);
            }
            if (!active)
            {
                selectUI.SetTrigger(DEACTIVATE);
                mainUI.SetActive(true);
            }
        } else if (uiObject == UIObject.InventoryUI)
        {
            inventoryUI.gameObject.SetActive(active);
        } else if (uiObject == UIObject.FilterUI)
        {
            filterUI.gameObject.SetActive(active);
        }
    }

    public void ToggleInventory(bool active)
    {
        inventoryUI.InitializeInventoryUI(active);
        SetUIObjectActive(active, UIObject.InventoryUI);
    }

    public void ToggleFilter(InventoryCategory category, bool active)
    {
        filterUI.InitializeFilter(category, active);
        SetUIObjectActive(active, UIObject.FilterUI);
    }

    public FarmItem AddItemToSelectContainer(GameObject itemObject)
    {
        GameObject newObject = Instantiate(itemObject);
        newObject.transform.SetParent(itemContainer, false);
        return newObject.GetComponent<FarmItem>();
    }

    public void EmptyContainer()
    {
        int childCount = itemContainer.childCount; // Must save value since Destroying changes it.
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(itemContainer.GetChild(i).gameObject);
        }
    }

    public void SetGold(int newGoldValue)
    {
        goldText.text = newGoldValue.ToString();
    }
}
