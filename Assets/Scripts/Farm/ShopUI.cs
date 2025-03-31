using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public enum ShopState
    {
        Main, Buy, Sell
    }

    [SerializeField] private FarmUI farmUI;
    [SerializeField] private TextMeshProUGUI shopMessageText;
    [SerializeField] private List<string> shopMessageList;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject mainContent;

    [Header("Buying")]
    [SerializeField] private GameObject buyContent;
    [SerializeField] private AnimatingTMP goldOwnedBuy;
    [SerializeField] private AnimatingTMP goldCost;
    [SerializeField] private InventoryItemSO shopItem;
    [SerializeField] private Image shopItemImage;
    [SerializeField] private TextMeshProUGUI shopItemName;
    [SerializeField] private TextMeshProUGUI shopItemDescription;
    [SerializeField] private TextMeshProUGUI shopItemPrice;
    [SerializeField] private Transform cartItemsTransform;
    [SerializeField] private Image purchaseButton;
    private Color purchaseColor;
    private List<InventoryItemSO> cartItems;
    private int cartAmount;

    [Header("Selling")]
    [SerializeField] private GameObject sellContent;
    [SerializeField] private Transform sellItems;
    [SerializeField] private AnimatingTMP goldOwnedSell;
    [SerializeField] private GameObject confirmDenyPopup;
    [SerializeField] private TextMeshProUGUI confirmDenyGold;
    private int currentSellAllGold;
    private List<InventoryItemUI> inventoryItemUIs;

    private Inventory inventory;
    private int goldAmount;

    private ShopState shopState;

    //TESTING
    [Header("TESTING")]
    [SerializeField] private GameObject shopOwner;

    private void Start()
    {
        inventory = GameManager.Instance.GetInventory();
        cartItems = new List<InventoryItemSO>();
        inventoryItemUIs = new List<InventoryItemUI>();
        goldAmount = inventory.GetGold();
        purchaseColor = purchaseButton.color;
        shopItemImage.sprite = shopItem.sprite;
        shopItemName.text = shopItem.itemName;
        shopItemDescription.text = shopItem.description;
        shopItemPrice.text = shopItem.dataA.ToString();
        shopState = ShopState.Main;
    }

    private void OnEnable()
    {
        shopOwner.SetActive(true);
        if (inventory != null)
        {
            goldAmount = inventory.GetGold();
            SetGold();
        }
    }

    private void OnDisable()
    {
        shopOwner.SetActive(false);
    }

    public void SelectMessage()
    {
        shopMessageText.text = shopMessageList[Random.Range(0, shopMessageList.Count)];
    }

    public void OpenBuy()
    {
        shopState = ShopState.Buy;
        mainContent.SetActive(false);
        buyContent.SetActive(true);
        goldOwnedBuy.InitializeValue(goldAmount);
        goldCost.InitializeValue(0);
    }

    public void CloseBuy()
    {
        shopState = ShopState.Main;
        EmptyCart();
        mainContent.SetActive(true);
        buyContent.SetActive(false);
    }

    public void OpenSell()
    {
        if (inventoryItemUIs.Count == 0)
        {
            inventoryItemUIs = new List<InventoryItemUI>(sellItems.GetComponentsInChildren<InventoryItemUI>());
        }
        ItemsWrapper sellItemsWrapper = inventory.FilterByCategory(InventoryCategory.Harvest);
        for (int i = 0; i < sellItemsWrapper.items.Count; i++) 
        {
            inventoryItemUIs[i].Populate(sellItemsWrapper.items[i], true);
            inventoryItemUIs[i].OnSellItem += ShopUI_OnSellItem;
        }
        shopState = ShopState.Sell;
        mainContent.SetActive(false);
        sellContent.SetActive(true);
        goldOwnedSell.InitializeValue(goldAmount);
    }

    private void ShopUI_OnSellItem(InventoryItem soldItem)
    {
        if (inventory.TryUseItem(soldItem.inventoryItemSO, 1, soldItem.quality))
        {
            ResetSell(true);
            inventory.ChangeGold(soldItem.inventoryItemSO.GetCost(soldItem.quality));
            SetGold();
        }
    }

    public void CloseSell()
    {
        ResetSell();
        shopState = ShopState.Main;
        mainContent.SetActive(true);
        sellContent.SetActive(false);
    }

    public void ResetSell(bool repopuplate = false)
    {
        foreach (InventoryItemUI itemSlot in inventoryItemUIs)
        {
            itemSlot.Empty();
            itemSlot.OnSellItem -= ShopUI_OnSellItem;
        }
        if (repopuplate)
        {
            ItemsWrapper sellItemsWrapper = inventory.FilterByCategory(InventoryCategory.Harvest);
            for (int i = 0; i < sellItemsWrapper.items.Count; i++)
            {
                inventoryItemUIs[i].Populate(sellItemsWrapper.items[i], true);
                inventoryItemUIs[i].OnSellItem += ShopUI_OnSellItem;
            }
        }
    }

    public void SellAll()
    {
        currentSellAllGold = 0;
        foreach (InventoryItemUI inventoryItemUI in inventoryItemUIs)
        {
            InventoryItem currentItem = inventoryItemUI.GetInventoryItem();
            if (!currentItem.IsNull())
            {
                currentSellAllGold += currentItem.inventoryItemSO.GetCost(currentItem.quality) * currentItem.quantity;
            }
        }
        confirmDenyPopup.SetActive(true);
        confirmDenyGold.text = currentSellAllGold.ToString();
    }

    public void CancelPopup()
    {
        confirmDenyGold.text = "0";
        confirmDenyPopup.SetActive(false);
    }

    public void ConfirmSellAll()
    {
        CancelPopup();
        inventory.ChangeGold(currentSellAllGold);
        SetGold();
        foreach (InventoryItemUI inventoryItemUI in inventoryItemUIs)
        {
            InventoryItem currentItem = inventoryItemUI.GetInventoryItem();
            if (!currentItem.IsNull())
            {
                inventory.TryUseItem(currentItem.inventoryItemSO, currentItem.quantity, currentItem.quality);
            }
        }
        ResetSell(true);
        currentSellAllGold = 0;
    }

    public void ExitShop()
    {
        switch (shopState) {
            case ShopState.Buy:
                CloseBuy();
                break;
            case ShopState.Sell:
                CloseSell();
                break;
            default:
                break;
        }
        farmUI.SetUIObjectActive(false, FarmUI.UISegment.ShopUI);
        GameManager.Instance.ResetGameState();
    }

    public void AddToCart()
    {
        if (cartItems.Count <= 7)
        {
            int index = cartItems.Count;
            Image itemImage = cartItemsTransform.GetChild(index).GetChild(0).GetComponent<Image>();
            itemImage.sprite = shopItem.sprite;
            itemImage.color = Color.white;
            cartItems.Add(shopItem);
            AddCartAmount(shopItem.dataA);
        }
    }

    public void RemoveFromCart(int index)
    {
        if (index < cartItems.Count && index >= 0)
        {
            Image itemImage = cartItemsTransform.GetChild(cartItems.Count - 1).GetChild(0).GetComponent<Image>();
            itemImage.color = Color.clear;
            itemImage.sprite = null;
            InventoryItemSO itemRemove = cartItems[index];
            AddCartAmount(-itemRemove.dataA);
            cartItems.RemoveAt(index);
            for (int i = index; i < cartItems.Count; i++) { 
                cartItemsTransform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = cartItems[i].sprite;
            }
        }
    }

    private void AddCartAmount(int amount)
    {
        cartAmount += amount;
        goldCost.AnimateChange(cartAmount);
        if (cartAmount > goldAmount)
        {
            goldCost.SetColor(Color.red);
            purchaseButton.color = Color.gray;
        } else
        {
            goldCost.SetColor(Color.white);
            purchaseButton.color = purchaseColor;
        }
    }

    public void EmptyCart()
    {
        int cartCount = cartItems.Count;
        for (int i = cartCount; i >= 0; i--) {
            RemoveFromCart(i);
        }
    }

    public void Purchase()
    {
        if (cartAmount < goldAmount && cartItems.Count > 0)
        {
            foreach (InventoryItemSO item in cartItems)
            {
                inventory.AddItem(new InventoryItem(item, 0, 1));
            }
            inventory.ChangeGold(-cartAmount);
            SetGold();
            EmptyCart();
        }
    }

    public void SetGold()
    {
        goldAmount = inventory.GetGold();
        if (shopState == ShopState.Buy) goldOwnedBuy.AnimateChange(goldAmount);
        else if (shopState == ShopState.Sell) goldOwnedSell.AnimateChange(goldAmount);
    }
}
