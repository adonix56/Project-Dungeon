using System;
using System.Collections.Generic;
using UnityEngine;

public class FarmGrid : MonoBehaviour
{
    [SerializeField] private int xSize;
    [SerializeField] private int ySize;
    [SerializeField] private Transform zeroZero;
    [SerializeField] private GameObject placementSquare;

    [Header("TESTING")]
    [SerializeField] private bool visualizeGrid;
    
    private Rect gridRect;
    private Dictionary<Vector2Int, bool> grid;

    private void Start()
    {
        SetupGrid();
    }

    private void SetupGrid()
    {
        grid = new Dictionary<Vector2Int, bool>();
        gridRect.x = zeroZero.position.x;
        gridRect.y = zeroZero.position.z;
        gridRect.width = xSize * 2;
        gridRect.height = ySize * 2;

        if (visualizeGrid)
        {
            for (int i = Mathf.RoundToInt(gridRect.x); i < Mathf.RoundToInt(gridRect.x) + gridRect.width; i += 2)
            {
                for (int j = Mathf.RoundToInt(gridRect.y); j < Mathf.RoundToInt(gridRect.x) + gridRect.height; j += 2)
                {
                    Instantiate(placementSquare, new Vector3(i, 0.01f, j), Quaternion.identity, transform);
                }
            }
        }
    }

    public void SetNewGridSize(int width, int length)
    {
        xSize = width;
        ySize = length;
        //TODO: Reset grid and set the placements of objects
    }

    public bool CanPlaceSpot(Vector2Int spot)
    {
        if (!gridRect.Contains(spot)) return false;
        return !grid.GetValueOrDefault(spot, false);
    }

    public bool TrySetSpot(Vector2Int spot, bool value)
    {
        if (!gridRect.Contains(spot)) return false;
        grid[spot] = value;
        return true;
    }
}
