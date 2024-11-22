using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] public float walkSpeed = 3.0f;
    [SerializeField] private float scurryMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] public float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.8f;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseXSensitivity = 2.0f;
    [SerializeField] private float mouseYSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 80.0f;


    [SerializeField] Transform Head;
    private CharacterController characterController;
    private NewPlayerInputHandler inputHandler;
    private Vector3 currentMovement;
    private float verticalRotation;

    private void Awake(){
        characterController = GetComponent<CharacterController>();
        inputHandler = NewPlayerInputHandler.Instance;
    }

    private void Update(){
        //Debug.Log($"Move Input: {inputHandler.MoveInput}, Look Input: {inputHandler.LookInput}, Jump: {inputHandler.JumpTriggered}, Scurry: {inputHandler.ScurryValue}");
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement(){
        float speed = walkSpeed * (inputHandler.ScurryValue > 0 ? scurryMultiplier : 1f);

        Vector3 inputDirection = new Vector3(inputHandler.MoveInput.x, 0f, inputHandler.MoveInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        worldDirection.Normalize();

        currentMovement.x = worldDirection.x * speed;
        currentMovement.z = worldDirection.z * speed;

        HandleJumping();
        characterController.Move(currentMovement * Time.deltaTime);

    }

    void HandleJumping(){
        if(characterController.isGrounded){
            currentMovement.y = -0.5f;
            if(inputHandler.JumpTriggered){
                currentMovement.y = jumpForce;
            }
        }else{
            currentMovement.y -= gravity * Time.deltaTime;
        }

    }

    void HandleRotation(){
        float mouseXRotation = inputHandler.LookInput.x * mouseXSensitivity;
        transform.Rotate(0,mouseXRotation,0);

        verticalRotation -= inputHandler.LookInput.y * mouseYSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        Head.transform.localRotation= Quaternion.Euler(verticalRotation, 0, 0);
    }

}
