using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Selectable SO", menuName = "Scriptable Objects/Selectable")]
public class SelectableSO : ScriptableObject
{
    public string UITitle;
    public FarmUI.UIObject UIObject;
    public List<GameObject> entryPrefab;
}
