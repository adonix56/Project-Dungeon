using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPopup : MonoBehaviour
{
    private const string HIDE = "Hide";

    public event Action<InventoryItemSO, int> OnSellClick;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI quantity;
    [SerializeField] private QualityDisplay qualityDisplay;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI sellPrice;
    [SerializeField] private GameObject sellButton;

    private Animator animator;
    private InventoryItemSO inventoryItemSO;
    private int quality;
    private PlayerController playerController;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GameManager.Instance.playerController;
        playerController.OnSelectPerformed += PlayerController_OnSelectPerformed;
    }

    private void PlayerController_OnSelectPerformed()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), playerController.GetTouchPosition()))
        {
            animator.SetTrigger(HIDE);
        }
    }

    public void Setup(InventoryItemSO inventoryItemSO, InventoryItem inventoryItem, bool includeSellButton = false)
    {
        this.inventoryItemSO = inventoryItemSO;
        quality = inventoryItem.quality;
        icon.sprite = inventoryItemSO.sprite;
        title.text = inventoryItemSO.itemName;
        quantity.text = inventoryItem.quantity.ToString();
        description.text = inventoryItemSO.description;
        qualityDisplay.SetQuality(inventoryItem.quality);
        sellPrice.text = inventoryItemSO.GetCost(inventoryItem.quality).ToString();
        if (includeSellButton) { 
            sellButton.SetActive(true);
        }
    }

    public void Sell()
    {
        OnSellClick?.Invoke(inventoryItemSO, quality);
    }

    public void Hide()
    {
        animator.SetTrigger(HIDE);
    }

    public void PublicDestroy()
    {
        Destroy(gameObject);
    }
}
