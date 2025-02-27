/* 
 * File: PlacementSquare.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/08/2025 
 */

using System;
using UnityEngine;

/// <summary>
/// Controls an individual placement square that dictates whether a Farm object can be placed in the grid.
/// </summary>
public class PlacementSquare : MonoBehaviour
{
    /// <summary>
    /// Event triggered when a the placement square's value has changed.
    /// </summary>
    public event Action<bool> OnValidChanged;

    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    /// <summary>
    /// Whether the placement square is valid (green).
    /// </summary>
    public bool IsValid { get; private set; }
    private Renderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        IsValid = true;
        CheckColor();
    }

    /// <summary>
    /// Set's the color to green (valid) or red (invalid).
    /// </summary>
    public void SetColor()
    {
        if (IsValid) meshRenderer.material = validMaterial;
        else meshRenderer.material = invalidMaterial;
    }

    /// <summary>
    /// Compares position to FarmGrid and sets value.
    /// </summary>
    public void CheckColor()
    {
        Vector3Int currentPosition = Vector3Int.RoundToInt(transform.position);
        Vector2Int currentPositionV2 = new Vector2Int(currentPosition.x, currentPosition.z);
        bool checkedSpot = FarmGrid.Instance.CanPlaceSpot(currentPositionV2);
        if (IsValid != checkedSpot)
        {
            IsValid = checkedSpot;
            OnValidChanged?.Invoke(IsValid);
            SetColor();
        }
    }

    /// <summary>
    /// Confirms placement and updates the FarmGrid.
    /// </summary>
    public void ConfirmPlacement()
    {
        Vector3Int currentPosition = Vector3Int.RoundToInt(transform.position);
        Vector2Int currentPositionV2 = new Vector2Int(currentPosition.x, currentPosition.z);
        FarmGrid.Instance.TrySetSpot(currentPositionV2, true);
    }
}
