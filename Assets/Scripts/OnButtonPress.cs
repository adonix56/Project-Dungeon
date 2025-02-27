/* 
 * File: OnButtonPress.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/06/2025 
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Simple dynamic class to run UnityEvents on inputs from the New Input System.
/// </summary>
public class OnButtonPress : MonoBehaviour
{
    /// <summary>
    /// InputAction to be determined in Unity Inspector.
    /// </summary>
    public InputAction action = null;
    /// <summary>
    /// UnityEvent triggered when input action is pressed.
    /// </summary>
    public UnityEvent OnPress = new UnityEvent();
    /// <summary>
    /// UnityEvent triggered when input action is released.
    /// </summary>
    public UnityEvent OnRelease = new UnityEvent();

    private void Awake()
    {
        action.started += Pressed;
        action.canceled += Released;
    }

    private void OnDestroy()
    {
        action.started -= Pressed;
        action.canceled -= Released;
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    /// <summary>
    /// Subscriber function to invoke Release event.
    /// </summary>
    /// <param name="obj">Input callback</param>
    private void Released(InputAction.CallbackContext obj)
    {
        OnRelease?.Invoke();
    }

    /// <summary>
    /// Subscriber function to invoke Pressed event.
    /// </summary>
    /// <param name="obj">Input callback</param>
    private void Pressed(InputAction.CallbackContext obj)
    {
        OnPress?.Invoke();
    }
}
