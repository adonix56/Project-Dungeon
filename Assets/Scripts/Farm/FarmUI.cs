using TMPro;
using UnityEngine;

public class FarmUI : MonoBehaviour
{
    private const string DEACTIVATE = "Deactivate";

    [Header("MainUI")]
    [SerializeField] private GameObject MainUI;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("SelectUI")]
    [SerializeField] private Animator selectUI;
    [SerializeField] private TextMeshProUGUI selectTitle;

    public enum UIObject {
        None, MainUI, SelectUI
    }

    public void SetUIObjectActive(SelectableSO obj, bool active)
    {
        Debug.Log($"J$ FarmUI {obj.UIObject.ToString()} {active}");
        if (obj.UIObject == UIObject.SelectUI) {
            if (active)
            {
                selectTitle.text = obj.UITitle;
                selectUI.gameObject.SetActive(true);
                MainUI.SetActive(false);
            }
            if (!active)
            {
                selectUI.SetTrigger(DEACTIVATE);
                MainUI.SetActive(true);
            }
        }
    }

    public void SetGold(int newGoldValue)
    {
        goldText.text = newGoldValue.ToString();
    }
}
