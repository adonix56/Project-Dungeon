using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmCancel : BaseObjectUI
{
    public event Action OnConfirm;
    public event Action OnCancel;

    [SerializeField] private Button confirm;
    [SerializeField] private Button cancel;

    protected override void Start()
    {
        base.Start();
        confirm.onClick.AddListener(() => { OnConfirm?.Invoke(); });
        cancel.onClick.AddListener(() => { OnCancel?.Invoke(); });
    }
}
