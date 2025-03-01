/* 
 * File: HarvestResult.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/28/2025 
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A sliding notification showing the results of a harvest.
/// </summary>
public class HarvestResult : MonoBehaviour
{
    [SerializeField] private Image harvestImage;
    [SerializeField] private TextMeshProUGUI harvestName;
    [SerializeField] private Slider qualitySlider;
    [SerializeField] private Image qualityFill;
    [SerializeField] private TextMeshProUGUI qualityText;
    [SerializeField] private GameObject critObj;
    [SerializeField] private Color minColor;
    [SerializeField] private Color lowColor;
    [SerializeField] private Color midColor;
    [SerializeField] private Color highColor;
    [SerializeField] private Color maxColor;


    /// <summary>
    /// Setup the values for the UI elements
    /// </summary>
    /// <param name="harvest">The harvested plant's InventoryItemSO.</param>
    /// <param name="quality">The quality of the harvested items.</param>
    /// <param name="crit">True if harvest resulted in a crit; otherwise, false.</param>
    public void SetupResult(InventoryItemSO harvest, int quality, bool crit)
    {
        harvestImage.sprite = harvest.sprite;
        harvestName.text = harvest.itemName;
        critObj.SetActive(crit);
        qualitySlider.value = (float) quality / 100f;
        qualityText.text = quality.ToString();
        Color changeColor = maxColor;
        if (quality < 100) changeColor = highColor;
        if (quality < 70) changeColor = midColor;
        if (quality < 30) changeColor = lowColor;
        if (quality == 0) changeColor = minColor;
        qualityFill.color = changeColor;
        qualityText.color = changeColor;
    }

    /// <summary>
    /// A public function to destroy itself. Allows destruction of object in Animator.
    /// </summary>
    public void PublicDestroy()
    {
        Destroy(gameObject);
    }
}
