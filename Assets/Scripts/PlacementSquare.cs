using UnityEngine;

public class PlacementSquare : MonoBehaviour
{
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    public bool IsValid { get; private set; }
    private Renderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        IsValid = true;
    }

    public void SetColor(bool valid)
    {
        IsValid = valid;
        if (valid) meshRenderer.material = validMaterial;
        else meshRenderer.material = invalidMaterial;
    }
}
