using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmItem : MonoBehaviour
{
    public event Action<FarmItem> OnClick;

    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI secondaryTimeLeftText;
    [SerializeField] private Image icon;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private GameObject secondaryActiveObject;
    [SerializeField] private GameObject secondaryInactiveObject;

    private GardenSoilItem currentGardenSoilItem;
    private bool gardenSoilActive;

    public bool IsSecondaryActive { get; private set; }

    private void Start()
    {
        IsSecondaryActive = true;
        GetComponent<Button>().onClick.AddListener(() => OnClick?.Invoke(this));
    }

    private void Update()
    {
        if (gardenSoilActive)
        {
            timeLeftText.text = currentGardenSoilItem.GetTimeLeft();
            timerSlider.value = currentGardenSoilItem.GetPercentageLeft();
        }
    }

    /*public void SetValues(string itemName = "", string itemDescription = "", string timeLeft = "", string secondaryTimeLeft = "", Sprite icon = null, float percentageTimeLeft = 0f)
    {
        this.itemName.text = itemName;
        //this.itemDescription.text = itemDescription;
        timeLeftText.text = timeLeft;
        secondaryTimeLeftText.text = secondaryTimeLeft;
        this.icon.sprite = icon;
        timerSlider.value = percentageTimeLeft;
    }*/

    public void PlantSeeds(GardenSoilItem gardenSoilItem)
    {
        currentGardenSoilItem = gardenSoilItem;
        InventoryItemSO harvestPlant = gardenSoilItem.inventoryItemSO.connectedInventoryItem;
        itemName.text = harvestPlant.itemName;
        timeLeftText.text = currentGardenSoilItem.GetTimeLeft();
        icon.sprite = harvestPlant.sprite;
        timerSlider.value = currentGardenSoilItem.GetPercentageLeft();
        gardenSoilActive = true;
    }

    public void SetDescription(string itemDescription)
    {
        this.itemDescription.text = itemDescription;
    }

    public void SetSliderValue(string timeLeft = "", float percentageTimeLeft = 0f)
    {
        timeLeftText.text = timeLeft;
        timerSlider.value = percentageTimeLeft;
    }

    public void SetSecondaryTimer(bool active, string secondaryTimeLeft = "")
    {
        secondaryActiveObject.SetActive(active);
        secondaryInactiveObject.SetActive(!active);
        secondaryTimeLeftText.text = secondaryTimeLeft;
    }
}
