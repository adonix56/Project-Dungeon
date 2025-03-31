using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject descriptionPrefab;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI quantity;

    public event Action<InventoryItem> OnSellItem;
    public event Action<InventoryItem> OnItemSelect;

    private DescriptionPopup currentDescriptionPopup;
    private InventoryItem currentInventoryItem = new InventoryItem();
    private bool withSellButton = false;
    private bool spawnDescription = true;

    public void Populate(InventoryItem inventoryItem, bool sell = false, bool description = true)
    {
        if (!inventoryItem.IsNull())
        {
            currentInventoryItem = inventoryItem;
            itemImage.color = Color.white;
            itemImage.sprite = inventoryItem.inventoryItemSO.sprite; 
            quantity.text = "";
            if (inventoryItem.quantity >= 9999) quantity.text = "9999";
            else if (inventoryItem.quantity != 1) quantity.text = inventoryItem.quantity.ToString();
            withSellButton = sell;
            spawnDescription = description;
        }
    }

    public void AllowDescriptions(bool allow)
    {
        spawnDescription = allow;
    }

    public void Empty()
    {
        currentInventoryItem = new InventoryItem();
        itemImage.color = Color.clear;
        itemImage.sprite = null;
        quantity.text = "";
        if (currentDescriptionPopup != null)
        {
            currentDescriptionPopup.PublicDestroy();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject && !currentInventoryItem.IsNull())
        {
            if (spawnDescription)
            {
                currentDescriptionPopup = Instantiate(descriptionPrefab).GetComponent<DescriptionPopup>();
                currentDescriptionPopup.transform.SetParent(transform, false);
                currentDescriptionPopup.OnSellClick += OnSellClick;
                currentDescriptionPopup.Setup(currentInventoryItem, withSellButton);
            }
            OnItemSelect?.Invoke(currentInventoryItem);
        }
    }

    private void OnSellClick(InventoryItem item)
    {
        OnSellItem?.Invoke(item);
    }

    public InventoryItem GetInventoryItem()
    {
        return currentInventoryItem;
    }
}
