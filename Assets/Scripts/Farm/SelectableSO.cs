/* 
 * File: SelectableSO.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/19/2025 
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a Scriptable Object for selectable farm objects.
/// </summary>
[CreateAssetMenu(fileName = "New Selectable SO", menuName = "Scriptable Objects/Selectable")]
public class SelectableSO : ScriptableObject
{
    /// <summary>
    /// The title of the selectable.
    /// </summary>
    public string title;
}
