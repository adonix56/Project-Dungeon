using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPopup : MonoBehaviour
{
    private const string HIDE = "Hide";

    public event Action<InventoryItem> OnSellClick;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI quantity;
    [SerializeField] private QualityDisplay qualityDisplay;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI sellPrice;
    [SerializeField] private GameObject sellButton;
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform bodyRectTransform;
    private InventoryItem inventoryItem;
    private int quality;
    private PlayerController playerController;

    private void Start()
    {
        playerController = GameManager.Instance.playerController;
        playerController.OnSelectPerformed += PlayerController_OnSelectPerformed;
    }

    private void PlayerController_OnSelectPerformed()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(bodyRectTransform, playerController.GetTouchPosition()))
        {
            Hide();
        }
        if (sellButton.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(sellButton.transform.GetChild(0).GetComponent<RectTransform>(), playerController.GetTouchPosition()))
        {
            Sell();
        }
    }

    public void Setup(InventoryItem inventoryItem, bool includeSellButton = false)
    {
        this.inventoryItem = inventoryItem;
        InventoryItemSO inventoryItemSO = inventoryItem.inventoryItemSO;
        quality = inventoryItem.quality;
        icon.sprite = inventoryItemSO.sprite;
        title.text = inventoryItemSO.itemName;
        quantity.text = inventoryItem.quantity.ToString();
        description.text = inventoryItemSO.description;
        qualityDisplay.SetQuality(inventoryItem.quality);
        sellPrice.text = inventoryItemSO.GetCost(inventoryItem.quality).ToString();
        sellButton.SetActive(includeSellButton);
    }

    public void Sell()
    {
        OnSellClick?.Invoke(inventoryItem); 
        playerController.OnSelectPerformed -= PlayerController_OnSelectPerformed;
        Destroy(gameObject);
    }

    public void Hide()
    {
        playerController.OnSelectPerformed -= PlayerController_OnSelectPerformed;
        animator.SetTrigger(HIDE);
    }

    public void PublicDestroy()
    {
        Destroy(gameObject);
    }
}
