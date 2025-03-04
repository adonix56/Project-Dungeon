using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private FarmUI farmUI;
    [SerializeField] private TextMeshProUGUI shopMessageText;
    [SerializeField] private List<string> shopMessageList;

    [Header("Buying")]
    [SerializeField] private GameObject buyContent;
    [SerializeField] private TextMeshProUGUI goldOwnedBuy;
    [SerializeField] private TextMeshProUGUI goldCost;
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
    [SerializeField] private TextMeshProUGUI goldOwnedSell;

    private Inventory inventory;
    private int goldAmount;

    //TESTING
    [Header("TESTING")]
    [SerializeField] private GameObject shopOwner;

    private void Start()
    {
        inventory = GameManager.Instance.GetInventory();
        cartItems = new List<InventoryItemSO>();
        goldAmount = inventory.GetGold();
        goldOwnedBuy.text = goldAmount.ToString();
        goldOwnedSell.text = goldAmount.ToString();
        purchaseColor = purchaseButton.color;
        shopItemImage.sprite = shopItem.sprite;
        shopItemName.text = shopItem.itemName;
        shopItemDescription.text = shopItem.description;
        shopItemPrice.text = shopItem.dataA.ToString();
    }

    private void OnEnable()
    {
        shopOwner.SetActive(true);
        if (inventory != null)
        {
            goldAmount = inventory.GetGold();
            goldOwnedBuy.text = goldAmount.ToString();
            goldOwnedSell.text = goldAmount.ToString();
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

    public void ExitShop()
    {
        EmptyCart();
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
        goldCost.text = cartAmount.ToString();
        if (cartAmount > goldAmount)
        {
            goldCost.color = Color.red;
            purchaseButton.color = Color.gray;
        } else
        {
            goldCost.color = Color.white;
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
                inventory.AddItem(item, new InventoryItem(0, 1));
            }
            inventory.ChangeGold(-cartAmount);
            goldAmount = inventory.GetGold();
            goldOwnedBuy.text = goldAmount.ToString();
            goldOwnedSell.text = goldAmount.ToString();
            EmptyCart();
        }
    }
}
