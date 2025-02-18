using UnityEngine;

public class FarmUI : MonoBehaviour
{
    [SerializeField] private GameObject backButton;

    public void BackButtonSetActive(bool active)
    {
        backButton.SetActive(active);
    }
}
