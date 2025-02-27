/* 
 * File: RadialFarm.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/08/2025 
 */

using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A set of radial buttons placed dynamically in the world.
/// </summary>
public class RadialFarm : BaseObjectUI
{
    /// <summary>
    /// Event triggered when the Move button is clicked.
    /// </summary>
    public event Action OnMove;
    /// <summary>
    /// Event triggered when the Rotate button is clicked.
    /// </summary>
    public event Action OnRotate;
    /// <summary>
    /// Event triggered when the Delete button is clicked.
    /// </summary>
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
