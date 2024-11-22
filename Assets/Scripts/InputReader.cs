using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, PlayerInput.IOnFootActions, PlayerInput.IMenuActions
{
    private PlayerInput _playerInput;

    private void OnEnable()
    {
        if (_playerInput == null)
        {
            _playerInput = new PlayerInput();

            _playerInput.OnFoot.SetCallbacks(this);
            _playerInput.Menu.SetCallbacks(this);

            SetOnFoot();
        }
    }

    public void SetOnFoot()
    {
        Debug.Log("OnFoot Set");
        _playerInput.OnFoot.Enable();
        _playerInput.Menu.Disable();
    }

    public void SetMenu()
    {
        Debug.Log("Menu Set");
        _playerInput.OnFoot.Disable();
        _playerInput.Menu.Enable();
    }

    public event Action<Vector2> MoveEvent;
    public event Action JumpEvent;
    public event Action JumpCancelledEvent;
    public event Action ScurryEvent;
    public event Action<Vector2> LookEvent;
    public event Action PauseEvent;
    public event Action ResumeEvent;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Canceled)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            JumpEvent?.Invoke();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            JumpCancelledEvent?.Invoke();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Canceled)
        {
            LookEvent?.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnScurry(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ScurryEvent?.Invoke();
        }

    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            PauseEvent?.Invoke();
            SetMenu();
        }
    }

    public void OnResume(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ResumeEvent?.Invoke();
            SetOnFoot();
        }
    }
}
