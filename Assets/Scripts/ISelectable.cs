using UnityEngine;

public interface ISelectable
{
    public void Select();

    public void SelectHold();

    public void StartMove();

    public void Move(Vector3 newLocation, bool newDragOffset);

    public void EndSelect();
}
