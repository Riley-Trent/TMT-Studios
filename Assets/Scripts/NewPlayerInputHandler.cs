using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "OnFoot";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string scurry = "Scurry";


    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction scurryAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public float ScurryValue { get; private set; }


    public static NewPlayerInputHandler Instance {get; private set; }

    private void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        scurryAction = playerControls.FindActionMap(actionMapName).FindAction(scurry);
        RegisterInputActions();
    }

    void RegisterInputActions(){
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += context => {LookInput = context.ReadValue<Vector2>(); Debug.Log($"Look Input: {LookInput}");};
        lookAction.canceled += context => LookInput = Vector2.zero;

        jumpAction.performed += context => JumpTriggered = true;
        jumpAction.canceled += context => JumpTriggered = false;

        scurryAction.performed += context => ScurryValue = context.ReadValue<float>();
        scurryAction.canceled += context => ScurryValue = 0f;
    }

    private void OnEnable(){
        Debug.Log("Enabling Input Actions");
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        scurryAction.Enable();
    }

    private void OnDisable(){
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        scurryAction.Disable();
    }
}
