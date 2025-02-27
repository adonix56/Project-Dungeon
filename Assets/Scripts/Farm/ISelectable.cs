/* 
 * File: GardenSoil.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/19/2025 
 */

using UnityEngine;

/// <summary>
/// Interface to ensure selectable farm objects include basic functions.
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Executed when object is selected.
    /// </summary>
    /// <returns>The selectable's corresponding scriptable object.</returns>
    public SelectableSO Select();

    /// <summary>
    /// Executed when select process is ended.
    /// </summary>
    public void EndSelect();

    /// <summary>
    /// Executed when object is select holded.
    /// </summary>
    public void SelectHold();

    /// <summary>
    /// Executed when ending select hold.
    /// </summary>
    public void EndSelectHold();

    /// <summary>
    /// Executed when attempting to move the object.
    /// </summary>
    public void StartMove();

    /// <summary>
    /// Move object to accepted new location.
    /// </summary>
    /// <param name="newLocation">New location to move the object</param>
    /// <param name="newDragOffset">True sets up the offset from the location to the cursor, False to change the location of the object.</param>
    public void Move(Vector3 newLocation, bool newDragOffset);
}
