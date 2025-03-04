using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldAmount;
    [SerializeField] private float goldChangeSpeed;

    private Inventory inventory;
    private bool goldChanging;
    private float currentGold;
    private int goldTarget;

    private void Start()
    {
        inventory = GameManager.Instance.GetInventory();
        inventory.OnGoldChange += OnGoldChange;
        goldAmount.text = inventory.GetGold().ToString();
    }

    private void Update()
    {
        if (goldChanging)
        {
            currentGold = Mathf.Lerp(currentGold, (float)goldTarget, goldChangeSpeed * Time.deltaTime);
            int goldCheck = Mathf.RoundToInt(currentGold);
            goldAmount.text = goldCheck.ToString();
            if (goldCheck == goldTarget)
            {
                goldChanging = false;
            }
        }
    }

    private void OnGoldChange(int newAmount)
    {
        currentGold = int.Parse(goldAmount.text);
        goldTarget = newAmount;
        goldChanging = true;
    }
}
