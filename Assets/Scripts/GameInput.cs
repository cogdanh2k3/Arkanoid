using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; set; }

    private InputActions inputAction;

    public event EventHandler OnMenuBtnPressed;

    private void Awake()
    {
        Instance = this;

        inputAction = new InputActions();
        inputAction.Enable();
        inputAction.Player.Menu.performed += Menu_performed;
    }

    private void Menu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnMenuBtnPressed?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        inputAction.Disable();
    }
}
    