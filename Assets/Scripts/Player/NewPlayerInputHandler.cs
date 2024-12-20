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
    [SerializeField] private string UIactionMapName = "Menu";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string scurry = "Scurry";
    [SerializeField] private string pause = "Pause";
    [SerializeField] private string attack = "Attack";

    [Header("UI Action Name References")]
    [SerializeField] private string resume = "Resume";


    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction scurryAction;
    private InputAction pauseAction;
    private InputAction attackAction;


    private InputAction resumeAction;



    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public float ScurryValue { get; private set; }
    public bool PauseTriggered { get; private set; }
    public bool attackTriggered { get; private set; }


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
        pauseAction = playerControls.FindActionMap(actionMapName).FindAction(pause);
        attackAction = playerControls.FindActionMap(actionMapName).FindAction(attack);

        resumeAction = playerControls.FindActionMap(UIactionMapName).FindAction(resume);
        
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

        pauseAction.performed += context => PauseTriggered = true;

        attackAction.performed += context => attackTriggered = true;
   
    }

    private void OnEnable(){
        Debug.Log("Enabling Input Actions");
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        scurryAction.Enable();
        pauseAction.Enable();
        attackAction.Enable();
    }

    private void OnDisable(){
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        scurryAction.Disable();
        pauseAction.Disable();
        attackAction.Disable();
    }
}
