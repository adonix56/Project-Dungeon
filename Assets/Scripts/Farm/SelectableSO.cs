using UnityEngine;

[CreateAssetMenu(fileName = "New Selectable SO", menuName = "Scriptable Objects/Selectable")]
public class SelectableSO : ScriptableObject
{
    public string UITitle;
    public FarmUI.UIObject UIObject;
    public GameObject entryPrefab;
}
