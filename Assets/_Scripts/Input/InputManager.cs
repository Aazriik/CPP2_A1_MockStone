using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>, InputSystem_Actions.IPlayerActions
{
    private InputSystem_Actions input;

    public event System.Action<Vector2> OnMoveEvent;
    public event System.Action<bool> OnJumpEvent;
    public event System.Action<Vector2> OnLookEvent;
    public event System.Action<bool> OnCrouchEvent;

    void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.SetCallbacks(this);
    }

    void OnEnable()
    {
        if (input == null)
        {
            input = new InputSystem_Actions();
            input.Player.SetCallbacks(this);
        }

        input.Enable();
    }

    void OnDisable()
    {
        // When Unity is exiting play mode or object is being destroyed,
        // input might already be null or invalid.
        if (input != null)
            input.Disable();
    }

    void OnDestroy()
    {
        if (input != null)
        {
            input.Dispose();
            input = null;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
            return;
        }

        OnMoveEvent?.Invoke(Vector2.zero);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            OnLookEvent?.Invoke(context.ReadValue<Vector2>());
            return;
        }

        OnLookEvent?.Invoke(Vector2.zero);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        OnJumpEvent?.Invoke(context.ReadValueAsButton());
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
            OnCrouchEvent?.Invoke(true);
        else if (context.canceled)
            OnCrouchEvent?.Invoke(false);
    }

    // Unused actions (leave empty)
    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
    public void OnSprint(InputAction.CallbackContext context) { }
}
