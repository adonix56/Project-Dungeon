using System;
using UnityEngine;

public class PlacementSquare : MonoBehaviour
{
    public event Action<bool> OnValidChanged;

    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    public bool IsValid { get; private set; }
    private Renderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        IsValid = true;
        CheckColor();
    }

    public void SetColor()
    {
        if (IsValid) meshRenderer.material = validMaterial;
        else meshRenderer.material = invalidMaterial;
    }

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

    public void ConfirmPlacement()
    {
        Vector3Int currentPosition = Vector3Int.RoundToInt(transform.position);
        Vector2Int currentPositionV2 = new Vector2Int(currentPosition.x, currentPosition.z);
        FarmGrid.Instance.TrySetSpot(currentPositionV2, true);
    }
}
