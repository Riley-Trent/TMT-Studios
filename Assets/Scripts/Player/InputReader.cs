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

    public event Action<Vector2> MoveEvent;
    public event Action JumpEvent;
    public event Action JumpCancelledEvent;
    public event Action ScurryEvent;
    public event Action<Vector2> LookEvent;
    public event Action InteractEvent;
    public event Action AttackEvent;
    public event Action StopAttackEvent;
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
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            InteractEvent?.Invoke();
        }

    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            AttackEvent?.Invoke();
        }else if(context.phase == InputActionPhase.Canceled)
        {
            StopAttackEvent?.Invoke();
        }

    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            PauseEvent?.Invoke();
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
