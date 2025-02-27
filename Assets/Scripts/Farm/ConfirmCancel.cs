/* 
 * File: ConfirmCancel.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/08/2025 
 */

using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A set of confirm and cancel buttons placed dynamically in the world.
/// </summary>
public class ConfirmCancel : BaseObjectUI
{
    /// <summary>
    /// Event triggered when the Confirm button is clicked.
    /// </summary>
    public event Action OnConfirm;
    /// <summary>
    /// Event triggered when the Cancel button is clicked.
    /// </summary>
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
