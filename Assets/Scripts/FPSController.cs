using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable{
    public void Interact();
    public void DisplayInteract();
    public void StopDisplay();
}
public class FPSController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] public float walkSpeed = 3.0f;
    [SerializeField] public float scurryMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] public float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.8f;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseXSensitivity = 2.0f;
    [SerializeField] private float mouseYSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 80.0f;
    [SerializeField] private float InteractRange = 80.0f;

    [Header("Misc")]
    [SerializeField] private float hideDelay = 1.0f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private InputReader input;
    [SerializeField] Transform Head;
    [SerializeField] private CinemachineVirtualCameraBase firstPersonCamera;
    [SerializeField] private CinemachineVirtualCameraBase thirdPersonCamera;
    [SerializeField] private GameObject characterModel;
    private CharacterController characterController;

    private Transform ActiveCameraTransform => isScurrying ? thirdPersonCamera.transform : firstPersonCamera.transform;


    private Vector3 currentMovement;
    private float mouseXRotation;
    private float verticalRotation;
    private bool isJumping = false;
    public bool isScurrying = false;
    private Vector2 moveInput;
    private IInteractable lastObject = null;


    private void Awake(){
        characterController = GetComponent<CharacterController>();
        input.MoveEvent += HandleMovement;
        input.ScurryEvent += ToggleScurry;
        input.JumpEvent += HandleJumping;
        input.JumpCancelledEvent += HandleJumpingCancelled;
        input.LookEvent += HandleRotation;
        input.InteractEvent += HandleInteract;
        HideCharacter();
    
    }

    private void Update(){
        animator.Update(Time.deltaTime);
        LookForInteract();
        Look();
        Jump();
        Move();
        
    }

    void ToggleScurry(){
        isScurrying = !isScurrying;
        if(isScurrying){
            SwitchToThirdPerson();
        }else{
            SwitchToFirstPerson();
        }
        animator.SetBool("IsScurrying", isScurrying);
    }



    void HandleMovement(Vector2 direction)
    {
        moveInput = direction;

        if (moveInput.magnitude > 0.1f)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }


    void HandleJumping(){
        isJumping = true;
        animator.SetBool("IsJumping", true);
    }

    void HandleJumpingCancelled(){
        isJumping = false;
        animator.SetBool("IsJumping", false);
    }

    void HandleRotation(Vector2 lookRotation){
        mouseXRotation = lookRotation.x * mouseXSensitivity;

        verticalRotation -= lookRotation.y * mouseYSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        
    }

    void Move(){
        float speed = walkSpeed * (isScurrying ? scurryMultiplier : 1f);

        //changed from this because animation rotation stumped me
        //Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 inputDirection = new Vector3(-moveInput.y, 0f, moveInput.x);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        worldDirection.Normalize();

        currentMovement.x = worldDirection.x * speed;
        currentMovement.z = worldDirection.z * speed;
        

        characterController.Move(currentMovement * Time.deltaTime);
    }
    void Jump(){
        if(characterController.isGrounded){
            currentMovement.y = -0.5f;
            if(isJumping){
                currentMovement.y = jumpForce;
            }
        }else{
            currentMovement.y -= gravity * Time.deltaTime;
        }
    }

    void Look()
    {
  
        transform.Rotate(0, mouseXRotation, 0);
        //changed from this because animation rotation stumped me
        //Head.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        Head.transform.localRotation = Quaternion.Euler(0, 0, verticalRotation);
    }

    void SwitchToThirdPerson()
    {
        if (thirdPersonCamera != null && firstPersonCamera != null)
        {
            thirdPersonCamera.Priority = 10;
            firstPersonCamera.Priority = 0;

            ShowCharacter();
        }
    }

    void SwitchToFirstPerson()
    {
        if (firstPersonCamera != null && thirdPersonCamera != null)
        {
            firstPersonCamera.Priority = 10;
            thirdPersonCamera.Priority = 0;

            StartCoroutine(HideCharacterAfterDelay());
        }
    }


    private bool TryRaycastInteractable(out IInteractable interactObj)
    {
        Ray ray = new Ray(ActiveCameraTransform.position, ActiveCameraTransform.forward);
        DebugRaycast(Head.position, ActiveCameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out interactObj))
            {
                return true;
            }
        }

        interactObj = null;
        return false;
    }
    void DebugRaycast(Vector3 origin, Vector3 direction)
    {
        Debug.DrawRay(origin, direction * InteractRange, Color.red, 0.1f);
    }
    void HandleInteract()
    {
        if (TryRaycastInteractable(out IInteractable interactObj))
        {
            interactObj.Interact();
        }
    }

    void LookForInteract()
    {
        if (TryRaycastInteractable(out IInteractable interactObj))
        {
            if (interactObj != lastObject)
            {
                lastObject?.StopDisplay();
                lastObject = interactObj;
                interactObj.DisplayInteract();
            }
        }
        else
        {
            lastObject?.StopDisplay();
            lastObject = null;
        }
    }

    void HideCharacter()
    {
        if (characterModel != null)
        {
            characterModel.SetActive(false);
        }
    }

    void ShowCharacter()
    {
        if (characterModel != null)
        {
            characterModel.SetActive(true);
        }
    }

    IEnumerator HideCharacterAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        HideCharacter();
    }

}
