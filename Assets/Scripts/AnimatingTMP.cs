using TMPro;
using UnityEngine;

public class AnimatingTMP : MonoBehaviour
{
    [SerializeField] private float valueChangeSpeed = 10f;

    private TextMeshProUGUI textValue;
    private Inventory inventory;
    private bool valueChanging;
    private float currentValue;
    private int goldTarget;

    public void InitializeValue(int startingValue)
    {
        valueChanging = false;
        currentValue = startingValue;
        if (textValue == null) textValue = GetComponent<TextMeshProUGUI>();
        textValue.text = currentValue.ToString();
    }

    private void Update()
    {
        if (valueChanging)
        {
            currentValue = Mathf.Lerp(currentValue, (float)goldTarget, valueChangeSpeed * Time.deltaTime);
            int goldCheck = Mathf.RoundToInt(currentValue);
            textValue.text = goldCheck.ToString();
            if (goldCheck == goldTarget)
            {
                valueChanging = false;
            }
        }
    }

    public void AnimateChange(int newAmount)
    {
        if (textValue == null) textValue = GetComponent<TextMeshProUGUI>();
        currentValue = int.Parse(textValue.text);
        goldTarget = newAmount;
        valueChanging = true;
    }

    public void SetColor(Color newColor)
    {
        if (textValue == null) textValue = GetComponent<TextMeshProUGUI>();
        textValue.color = newColor;
    }
}
