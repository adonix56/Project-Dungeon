using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmItem : MonoBehaviour
{
    public event Action OnClick;

    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI secondaryTimeLeftText;
    [SerializeField] private Image icon;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private GameObject secondaryActiveObject;
    [SerializeField] private GameObject secondaryInactiveObject;

    public bool IsSecondaryActive { get; private set; }

    private void Start()
    {
        IsSecondaryActive = true;
        GetComponent<Button>().onClick.AddListener(() => OnClick?.Invoke());
    }

    public void SetValues(string itemName = "", string itemDescription = "", string timeLeft = "", string secondaryTimeLeft = "", Sprite icon = null, float percentageTimeLeft = 0f)
    {
        this.itemName.text = itemName;
        this.itemDescription.text = itemDescription;
        timeLeftText.text = timeLeft;
        secondaryTimeLeftText.text = secondaryTimeLeft;
        this.icon.sprite = icon;
        timerSlider.value = percentageTimeLeft;
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
