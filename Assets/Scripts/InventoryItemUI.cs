using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject descriptionPrefab;

    public event Action<InventoryItemSO, int> OnSellItem;

    private DescriptionPopup currentDescriptionPopup;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            currentDescriptionPopup = Instantiate(descriptionPrefab).GetComponent<DescriptionPopup>();
            currentDescriptionPopup.transform.SetParent(transform, false);
            currentDescriptionPopup.OnSellClick += OnSellClick;
            Debug.Log("J$ InventoryItemUI OpenDescription?");
        }
    }

    private void OnSellClick(InventoryItemSO inventoryItemSO, int quality)
    {
        
    }
}
