using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>, InputSystem_Actions.IPlayerActions
{
    private InputSystem_Actions input;

    public event System.Action<Vector2> OnMoveEvent;
    public event System.Action<bool> OnJumpEvent;
    public event System.Action<bool> OnInteractEvent;
    public event System.Action<bool> OnCrouchEvent;
    public event System.Action<bool> OnSprintEvent;
    public event System.Action<bool> OnPauseEvent;

    protected override void Awake()
    {
        base.Awake();

        input = new InputSystem_Actions();
        input.Player.SetCallbacks(this);
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();

    public void SetPlayerControlsActive(bool active)
    {
        if (active)
        {
            input.UI.Disable();
            input.Player.Enable();
        }
        else
        {
            input.Player.Disable();
            input.UI.Enable();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
        else if (context.canceled)
            OnMoveEvent?.Invoke(Vector2.zero);
    }

    // Unused actions required by the IPlayerActions Interface
    public void OnLook(InputAction.CallbackContext context) { }
    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) OnInteractEvent?.Invoke(true);
        else if (context.canceled) OnInteractEvent?.Invoke(false);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed) OnCrouchEvent?.Invoke(true);
        else if (context.canceled) OnCrouchEvent?.Invoke(false);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) OnJumpEvent?.Invoke(true);
        else if (context.canceled) OnJumpEvent?.Invoke(false);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed) OnSprintEvent?.Invoke(true);
        else if (context.canceled) OnSprintEvent?.Invoke(false);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) OnPauseEvent?.Invoke(true);
    }
}