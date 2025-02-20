using UnityEngine;

public interface ISelectable
{
    public SelectableSO Select();

    public void EndSelect();

    public void SelectHold();

    public void EndSelectHold();

    public void StartMove();

    public void Move(Vector3 newLocation, bool newDragOffset);
}
