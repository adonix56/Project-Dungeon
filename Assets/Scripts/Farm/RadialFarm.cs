using System;
using UnityEngine;
using UnityEngine.UI;

public class RadialFarm : BaseObjectUI
{
    public event Action OnMove;
    public event Action OnRotate;
    public event Action OnDelete;

    [SerializeField] private Button move;
    [SerializeField] private Button rotate;
    [SerializeField] private Button delete;

    protected override void Start()
    {
        base.Start();
        move.onClick.AddListener(() => { OnMove?.Invoke(); });
        rotate.onClick.AddListener(() => { OnRotate?.Invoke(); });
        delete.onClick.AddListener(() => { OnDelete?.Invoke(); });
    }

    protected override void Update()
    {
        base.Update();
    }
}
