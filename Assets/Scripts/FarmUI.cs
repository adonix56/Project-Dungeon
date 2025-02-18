using UnityEngine;

public class FarmUI : MonoBehaviour
{
    private const string DEACTIVATE = "Deactivate";

    [SerializeField] private Animator selectUI;

    public enum UIObject {
        None, SelectUI
    }

    public void SetUIObjectActive(UIObject obj, bool active)
    {
        if (obj == UIObject.SelectUI) { 
            if (active) selectUI.gameObject.SetActive(true);
            if (!active) selectUI.SetTrigger(DEACTIVATE);
        }
    }
}
