/* 
 * File: FarmGrid.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/08/2025 
 */

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the overall grid placements of farm objects.
/// </summary>
public class FarmGrid : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of FarmGrid.
    /// </summary>
    public static FarmGrid Instance { get; private set; }
    [SerializeField] private int xSize;
    [SerializeField] private int ySize;
    [SerializeField] private Transform zeroZero;
    [SerializeField] private GameObject placementSquare;

    [Header("TESTING")]
    [SerializeField] private bool visualizeGrid;
    [SerializeField] private Material gridSquareMaterial;
    
    private Rect gridRect;
    private Dictionary<Vector2Int, bool> grid;

    private void Awake()
    {
        // Singleton logic
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this; 
        gridRect.x = zeroZero.position.x;
        gridRect.y = zeroZero.position.z;
        gridRect.width = xSize * 2;
        gridRect.height = ySize * 2;
        grid = new Dictionary<Vector2Int, bool>();
    }

    private void Start()
    {
        SetupGrid();
    }

    /// <summary>
    /// Spawns placement squares to visually show the bounds of the grid.
    /// </summary>
    private void SetupGrid()
    {
        if (visualizeGrid)
        {
            for (int i = Mathf.RoundToInt(gridRect.x); i < Mathf.RoundToInt(gridRect.x) + gridRect.width; i += 2)
            {
                for (int j = Mathf.RoundToInt(gridRect.y); j < Mathf.RoundToInt(gridRect.y) + gridRect.height; j += 2)
                {
                    MeshRenderer mRenderer = Instantiate(placementSquare, new Vector3(i, 0.01f, j), Quaternion.identity, transform).GetComponent<MeshRenderer>();
                    mRenderer.material = gridSquareMaterial;
                }
            }
        }
    }

    /// <summary>
    /// Sets new values of the grid.
    /// </summary>
    /// <param name="width">New width to be set.</param>
    /// <param name="length">New height to be set.</param>
    public void SetNewGridSize(int width, int length)
    {
        xSize = width;
        ySize = length;
        //TODO: Reset grid and set the placements of objects
    }

    /// <summary>
    /// Checks if a spot on the grid can be placed.
    /// </summary>
    /// <param name="spot">A Vector2Int location to check if placement is available.</param>
    /// <returns>True if the spot can be placed; otherwise, false.</returns>
    public bool CanPlaceSpot(Vector2Int spot)
    {
        if (!gridRect.Contains(spot)) return false;
        return !grid.GetValueOrDefault(spot, false);
    }

    /// <summary>
    /// Attempts to set a new value to a location on the grid.
    /// </summary>
    /// <param name="spot">A Vector2Int location to set a new value.</param>
    /// <param name="occupied">True if the spot is to be occupied; false for available.</param>
    /// <returns>True if the spot can be changed; otherwise, false.</returns>
    public bool TrySetSpot(Vector2Int spot, bool occupied)
    {
        if (!gridRect.Contains(spot)) return false;
        if (!grid.ContainsKey(spot)) grid.Add(spot, occupied);
        else grid[spot] = occupied;
        return true;
    }
}
