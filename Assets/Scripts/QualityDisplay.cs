using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QualityDisplay : MonoBehaviour
{
    [SerializeField] private Slider qualitySlider;
    [SerializeField] private Image qualityFill;
    [SerializeField] private TextMeshProUGUI qualityText;
    [SerializeField] private Color minColor;
    [SerializeField] private Color lowColor;
    [SerializeField] private Color midColor;
    [SerializeField] private Color highColor;
    [SerializeField] private Color maxColor;

    public void SetQuality(int quality)
    {
        qualitySlider.value = (float)quality / 100f;
        qualityText.text = quality.ToString();
        Color changeColor = maxColor;
        if (quality < 100) changeColor = highColor;
        if (quality < 70) changeColor = midColor;
        if (quality < 30) changeColor = lowColor;
        if (quality == 0) changeColor = minColor;
        qualityFill.color = changeColor;
        qualityText.color = changeColor;
    }
}
