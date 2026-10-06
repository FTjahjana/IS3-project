using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerActions : MonoBehaviour
{

    public PlayerInput playerInput; 
    private List<InputAction> activePlayerActions = new List<InputAction>();
    public bool playerActive;

    [Header("Move")]
    public InputAction moveAction; private Vector2 moveInput;
    public CharacterController characterController;
    public float moveSpeed = 3; [SerializeField] float baseMoveSpeed = 3;
    public enum MoveMode {Classic = 0, ThirdPerson =1, SideScroller = 2} public MoveMode moveMode = MoveMode.Classic;

    [Header("Sprint")]
    public InputAction sprintAction;
    public float maxSprintStamina = 5f;
    public float sprintRegenRate = 2f;
    private float sprintStamina;

    [Header("Look")]
    public InputAction lookAction; public bool canLook = true;
    public GameObject[] playerCameras; 
    /*Element 0 → First Person Camera
    Element 1 → Third Person Camera
    Element 2 → SideScroller Camera*/
    [SerializeField] private float lookSensitivityDefault = .5f; public float lookSensitivity;
    public float upDownRange = 80.0f;
    private float yRotation;

    [Header("Jump")]
    public InputAction jumpAction;
    private float jumpHeight = 1.5f;
    private float gravity = -9.81f;
    private Vector3 verticalVelocity;

    [Header("User Interaction Actions")]
    public InputAction attackAction, interactAction, cursorToggleAction;
    public GameObject crosshairObj; public bool cursorLocked = true; 
    public bool cursorToggleable = true;

    [Header("UI Interaction Actions")]
    public InputAction previousAction, nextAction;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        //Movement
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        jumpAction = playerInput.actions.FindAction("Jump");

        //Movement Modifiers
        sprintAction = playerInput.actions.FindAction("Sprint");

        //Interaction
        attackAction = playerInput.actions.FindAction("Attack");
        interactAction = playerInput.actions.FindAction("Interact");
        cursorToggleAction = playerInput.actions.FindAction("Cursor Toggle");

        //UI
        previousAction = playerInput.actions.FindAction("Previous");
        nextAction = playerInput.actions.FindAction("Next");

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        sprintStamina = maxSprintStamina;
        lookSensitivity = lookSensitivityDefault;

        activePlayerActions.AddRange(new InputAction[]{
            //Movement
            moveAction, jumpAction,
            //Movement Modifiers
            sprintAction,
            //Interaction
            attackAction, interactAction, cursorToggleAction
        }); 
        PlayerActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        if (playerActive)
        {
            moveInput = moveAction.ReadValue<Vector2>();
            Sprint(); Move();
            if (canLook) Look();

            if (cursorToggleable && cursorToggleAction.WasPressedThisFrame())
            SetCursorMode(!cursorLocked);
        } 
    }

    public void PlayerActive(bool enabled)
    {
        playerActive = enabled;
        foreach (var a in activePlayerActions)
        {
            if (enabled) a.Enable();
            else a.Disable();
        }

        SetCursorMode(!enabled);       
    }

    public void SetCursorMode(bool locked)
    {
        cursorLocked = locked;

        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lookAction.Enable(); crosshairObj.SetActive(true);
            canLook = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true; crosshairObj.SetActive(false);
            lookAction.Disable();
            canLook = false;
        }
    }

    void Move()
    {
        Vector3 movement = transform.rotation * new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;

        if (moveMode == MoveMode.SideScroller)movement.z = 0;

        // jumping
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f;

            if (jumpAction.WasPressedThisFrame())
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        verticalVelocity += gravity * Time.deltaTime;

        //final
        movement.y = verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
    }

    void Sprint()
    {
        bool sprinting = sprintAction.IsPressed() /*&& sprintStamina > 0*/ 
                            && moveInput.sqrMagnitude > 0.01f;

        moveSpeed = sprinting ? baseMoveSpeed * 2.5f : baseMoveSpeed;
        /*
        sprintStamina += (sprinting ? -1 : sprintRegenRate) * Time.deltaTime;
        sprintStamina = Mathf.Clamp(sprintStamina, 0, maxSprintStamina);
        */
    }

    void Look()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        float xRotation = lookInput.x * lookSensitivity;
        transform.Rotate(0, xRotation, 0);

        yRotation -= lookInput.y * lookSensitivity;
        yRotation = Mathf.Clamp(yRotation, -upDownRange, upDownRange);
        playerCamera.transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
    }

    public void SpawnPlayerAt(Vector3 pos, float yRot)
    {
        //do a SpawnPlayerAt({start position},{start y rotation}) in Event Manager
        transform.position = pos; transform.eulerAngles = new Vector3(0, yRot, 0);
    }

    public void SetMoveMode(MoveMode mode)
    {
        moveMode = mode;
        
        for (int i = 0; i < playerCameras.Length; i++){
            playerCameras[i].SetActive(i == (int)moveMode);}

        switch (moveMode)
        {
            case MoveMode.Classic:
            cursorToggleable = true;
                break;

            case MoveMode.SideScroller:
                SetCursorMode(false); cursorToggleable = false;
                break;
        }
    }

}