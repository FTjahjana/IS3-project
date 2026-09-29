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
    private List<InputAction> activityActions = new List<InputAction>();

    [Header("Move")]
    public InputAction moveAction; public CharacterController characterController;
    public float moveSpeed = 3; [SerializeField] float baseMoveSpeed = 3;

    [Header("Sprint")]
    public InputAction sprintAction;
    public float maxSprintStamina = 5f;
    public float sprintRegenRate = 2f;
    private float sprintStamina;

    [Header("Look")]
    public InputAction lookAction; public bool canLook = true;
    public GameObject playerCamera;
    [SerializeField] private float lookSensitivityDefault = .5f; public float lookSensitivity;
    public float upDownRange = 80.0f;
    private float yRotation;

    [Header("Jump")]
    public InputAction jumpAction;
    public float jumpSpeed = 3.0f;
    public float gravity = 10.0f;
    private Vector3 movingDirection = Vector3.zero;

    [Header("Fly")]
    public bool flyingAllowed = true, isFlying; //public CharAnimations charAnimations;
    public event Action<bool> OnFlyingToggled; public bool G_Effective = true;

    [Header("User Interaction Actions")]
    public InputAction attackAction, interactAction;
    public GameObject cursorObj; public bool cursorLocked = true;

    [Header("UI Interaction Actions")]
    public InputAction previousAction, nextAction;

    [Header("Stored Positions")]
    public Vector3 startTutPos; public float startTutRotY;


    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        //Movement
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        jumpAction = playerInput.actions.FindAction("Jump");
            jumpAction.performed += OnJumpFlyPerformed;

        //Movement Modifiers
        sprintAction = playerInput.actions.FindAction("Sprint");

        //Interaction
        attackAction = playerInput.actions.FindAction("Attack");
        interactAction = playerInput.actions.FindAction("Interact");

        //UI
        previousAction = playerInput.actions.FindAction("Previous");
        nextAction = playerInput.actions.FindAction("Next");

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        sprintStamina = maxSprintStamina;
        lookSensitivity = lookSensitivityDefault;
 
        activityActions.AddRange( new InputAction[]{
            
            //Movement
            moveAction, lookAction, jumpAction, 

            //Movement Modifiers
            sprintAction,

            //Interaction
            attackAction, interactAction,

            //UI
            previousAction, nextAction
        
        });

        //temp:
        transform.position = startTutPos; transform.eulerAngles = new Vector3(0,startTutRotY,0);
    }


    // Update is called once per frame
    void Update()
    {
        Move();
        Sprint();
        if (canLook) Look();
        //cursorToggle();

        ApplyGravity();

    }

    public void ToggleActions(bool thing)
    {   
        foreach (var a in activityActions)
            {if (thing) a.Enable(); else a.Disable();}
            
        canLook = thing;

        if (thing)
        {
            Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            cursorObj.SetActive(cursorLocked); 
            if (!cursorLocked) lookAction.Disable(); else lookAction.Enable();
        }
        else
        {
            lookAction.Disable();
            Cursor.lockState = CursorLockMode.None;
            cursorObj.SetActive(false); 
        }    
    }

    void Move()
    {
        Vector3 moveInput = moveAction.ReadValue<Vector3>() * moveSpeed;
        Vector3 horizontalMovement = new Vector3(moveInput.x, 0, moveInput.z);
        horizontalMovement = transform.rotation * horizontalMovement;

        //flight
        if (isFlying)
        {
            float verticalInput = -moveInput.y;
            movingDirection.y += verticalInput * moveSpeed * Time.deltaTime;
            movingDirection.y = Mathf.Clamp(movingDirection.y, -moveSpeed, moveSpeed);

            Vector3 move = horizontalMovement + Vector3.up * movingDirection.y;
            characterController.Move(move * Time.deltaTime);
        } else {
            //move the character
            characterController.Move(horizontalMovement * Time.deltaTime);
        }
    }

    void Sprint()
    {
        if (isFlying) return;
        bool sprinting = sprintAction.IsPressed() /*&& sprintStamina > 0*/ &&
                        (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);

        moveSpeed = sprinting ? baseMoveSpeed * 2.5f : baseMoveSpeed;
        /*
        sprintStamina += (sprinting ? -1 : sprintRegenRate) * Time.deltaTime;
        sprintStamina = Mathf.Clamp(sprintStamina, 0, maxSprintStamina);
        */
    }

    void Look()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        float xRotation = lookInput.x * lookSensitivity * Time.deltaTime * 100;
        transform.Rotate(0, xRotation, 0);

        yRotation -= lookInput.y * lookSensitivity * Time.deltaTime * 100;
        yRotation = Mathf.Clamp(yRotation, -upDownRange, upDownRange);
        playerCamera.transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
    }

    public void cursorToggle(bool fromOuterScript = false)
    {
        Debug.Log($"<color=orange> You have somehow triggered a cursorToggle. </color>");
        /*
        if (cursorToggleAction.WasPressedThisFrame() || fromOuterScript)
        {
            bool isActive = cursorObj.activeSelf;
            cursorObj.SetActive(!isActive);

            if (isActive)
            { Cursor.lockState = CursorLockMode.None; lookAction.Disable(); cursorLocked = false;}
            else { Cursor.lockState = CursorLockMode.Locked; lookAction.Enable(); cursorLocked = true;}
        }*/
    }

    private void OnJumpFlyPerformed(InputAction.CallbackContext context)
    {
        Debug.Log($"[JUMPFLY DEBUG] Interaction = {context.interaction?.GetType().Name}");

        if (context.interaction is TapInteraction)
        {
            if (characterController.isGrounded)
            {
                if (isFlying) return;
                movingDirection.y = jumpSpeed;
            }
        }
        else if (context.interaction is MultiTapInteraction)
        {   if (!flyingAllowed) return;
        //charAnimations.WingAnimCall();
            
            if (isFlying)
            {
                Debug.Log("flying off");
                isFlying = false; 
            }
            else
            {
                Debug.Log("flying initiated");
                isFlying = true; 
                movingDirection.y += jumpSpeed*2;
            }

            OnFlyingToggled?.Invoke(isFlying);
        }
    }

    void ApplyGravity()
    {
        if (!isFlying)
        {
            if (!characterController.isGrounded)
                movingDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            if (G_Effective) movingDirection.y -= (gravity * 0.05f) * Time.deltaTime;
        }

        characterController.Move(movingDirection * Time.deltaTime);
    }

}