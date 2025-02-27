/* 
 * File: FarmItemUI.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/25/2025 
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A singular UI element inside a SelectUI container.
/// </summary>
public class FarmItemUI : MonoBehaviour
{
    /// <summary>
    /// Event triggered when the farm item is clicked.
    /// </summary>
    public event Action<FarmItemUI> OnClick;

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

    /// <summary>
    /// Gets a value indicating whether the secondary action is active.
    /// </summary>
    public bool IsSecondaryActive { get; private set; }

    private void Start()
    {
        IsSecondaryActive = true;
        GetComponent<Button>().onClick.AddListener(() => OnClick?.Invoke(this));
    }

    private void Update()
    {
        // Update GUI when active
        if (gardenSoilActive)
        {
            timeLeftText.text = currentGardenSoilItem.GetTimeLeft();
            timerSlider.value = currentGardenSoilItem.GetPercentageLeft();
        }
    }

    /// <summary>
    /// When planting seeds, this function updates the state of the UI elements.
    /// </summary>
    /// <param name="gardenSoilItem"></param>
    public void PlantSeeds(GardenSoilItem gardenSoilItem)
    {
        currentGardenSoilItem = gardenSoilItem;
        InventoryItemSO harvestPlant = gardenSoilItem.seedItemSO.connectedInventoryItem;
        itemName.text = harvestPlant.itemName;
        timeLeftText.text = currentGardenSoilItem.GetTimeLeft();
        icon.sprite = harvestPlant.sprite;
        timerSlider.value = currentGardenSoilItem.GetPercentageLeft();
        gardenSoilActive = true;
    }

    /// <summary>
    /// Sets the description text of the farm item.
    /// </summary>
    /// <param name="itemDescription">The description text to set.</param>
    public void SetDescription(string itemDescription)
    {
        this.itemDescription.text = itemDescription;
    }

    /// <summary>
    /// Sets the time left text and slider value.
    /// </summary>
    /// <param name="timeLeft">The time left text to set. Default is "".</param>
    /// <param name="percentageTimeLeft">The percentage of time left to set. Default is 0.</param>
    public void SetSliderValue(string timeLeft = "", float percentageTimeLeft = 0f)
    {
        timeLeftText.text = timeLeft;
        timerSlider.value = percentageTimeLeft;
    }

    /// <summary>
    /// Sets the secondary action state and the secondary time left text.
    /// </summary>
    /// <param name="active">Whether the secondary action is active.</param>
    /// <param name="secondaryTimeLeft">The secondary time left text to set. Default is "".</param>
    public void SetSecondaryTimer(bool active, string secondaryTimeLeft = "")
    {
        secondaryActiveObject.SetActive(active);
        secondaryInactiveObject.SetActive(!active);
        secondaryTimeLeftText.text = secondaryTimeLeft;
    }
}
