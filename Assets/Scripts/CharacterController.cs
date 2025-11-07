using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class CompleteFPCC : MonoBehaviour
{
    // Components
    private CharacterController controller;
    private Transform playerTx;
    
    [Header("References")]
    public Transform cameraTx;
    public Transform playerGFX;
    
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2.5f;
    
    [Header("Jump Settings")]
    public float jumpHeight = 1.5f;
    public float gravityValue = 9.81f;
    
    [Header("Crouch Settings")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public float crouchTransitionSpeed = 10f;
    
    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float verticalLookLimit = 80f;
    
    [Header("Layer Mask")]
    public LayerMask castingMask;
    
    // Private variables
    private Vector3 moveDirection;
    private float verticalVelocity;
    private bool isGrounded;
    private bool isCrouching;
    private float cameraVerticalAngle = 0f;
    
    // Input variables
    private float inputMoveX, inputMoveY;
    private float inputLookX, inputLookY;
    private bool inputKeyRun, inputKeyCrouch;
    private bool inputKeyJump;

    // ===== NEW: ARM ANIMATION VARIABLES =====
    [Header("Procedural Arm Animation")]
    public Transform leftArm;
    public Transform rightArm;
    public float idleBreathAmount = 2f;
    public float walkBobAmount = 10f;
    public float armSwayAmount = 5f;
    private float breathTimer;
    private float walkBobTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerTx = transform;
        
        // Lock cursor for FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
         if (!leftArm || !rightArm)
    {
        CreateTestArms();
    }
        // Initialize arm positions if they exist
        if (leftArm) leftArm.localPosition = new Vector3(-0.3f, -0.4f, 0.5f);
        if (rightArm) rightArm.localPosition = new Vector3(0.3f, -0.4f, 0.5f);
    }

    void Update()
    {
        HandleInput();
        HandleLook();
        HandleMovement();
        HandleCrouch();
        HandleJump();
        ApplyGravity();
        
        // ===== NEW: CALL ARM ANIMATION =====
        HandleProceduralAnimation();
    }
    void CreateTestArms()
{
    // Create a visible red cube for left arm
    GameObject left = GameObject.CreatePrimitive(PrimitiveType.Cube);
    left.name = "TEST_LeftArm";
    left.transform.SetParent(cameraTx);
    left.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    left.transform.localPosition = new Vector3(-0.5f, -0.4f, 0.5f);
    left.GetComponent<Renderer>().material.color = Color.red;
    
    // Create a visible blue cube for right arm
    GameObject right = GameObject.CreatePrimitive(PrimitiveType.Cube);
    right.name = "TEST_RightArm";
    right.transform.SetParent(cameraTx);
    right.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    right.transform.localPosition = new Vector3(0.5f, -0.4f, 0.5f);
    right.GetComponent<Renderer>().material.color = Color.blue;
    
    // Assign them to the script
    leftArm = left.transform;
    rightArm = right.transform;
    
    Debug.Log("✅ TEST ARMS CREATED - Check if you see red/blue cubes!");
}
    void HandleInput()
    {
        inputMoveX = Input.GetAxis("Horizontal");
        inputMoveY = Input.GetAxis("Vertical");
        inputLookX = Input.GetAxis("Mouse X");
        inputLookY = Input.GetAxis("Mouse Y");
        inputKeyRun = Input.GetKey(KeyCode.LeftShift);
        inputKeyCrouch = Input.GetKey(KeyCode.LeftControl);
        inputKeyJump = Input.GetKeyDown(KeyCode.Space);
    }

    void HandleLook()
    {
        // Horizontal rotation (player body)
        playerTx.Rotate(0, inputLookX * mouseSensitivity, 0);
        
        // Vertical rotation (camera)
        cameraVerticalAngle -= inputLookY * mouseSensitivity;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -verticalLookLimit, verticalLookLimit);
        cameraTx.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
    }

    void HandleMovement()
    {
        Vector3 forward = playerTx.TransformDirection(Vector3.forward);
        Vector3 right = playerTx.TransformDirection(Vector3.right);
        
        float speed = isCrouching ? crouchSpeed : (inputKeyRun ? runSpeed : walkSpeed);
        
        float curSpeedX = speed * inputMoveY;
        float curSpeedY = speed * inputMoveX;
        
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
    }

    void HandleCrouch()
    {
        if (inputKeyCrouch && !isCrouching)
        {
            isCrouching = true;
            controller.height = crouchingHeight;
            controller.center = new Vector3(0, crouchingHeight / 2, 0);
            if (playerGFX) playerGFX.localScale = new Vector3(1, 0.5f, 1);
        }
        else if (!inputKeyCrouch && isCrouching && CanStandUp())
        {
            isCrouching = false;
            controller.height = standingHeight;
            controller.center = new Vector3(0, standingHeight / 2, 0);
            if (playerGFX) playerGFX.localScale = Vector3.one;
        }
    }

    bool CanStandUp()
    {
        // Check if there's headroom to stand up
        RaycastHit hit;
        if (Physics.SphereCast(playerTx.position, controller.radius, Vector3.up, out hit, standingHeight - crouchingHeight, castingMask))
        {
            return false;
        }
        return true;
    }

    void HandleJump()
    {
        isGrounded = controller.isGrounded;
        
        if (isGrounded && inputKeyJump)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * 2 * gravityValue);
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravityValue * Time.deltaTime;
        }
        else if (verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }
        
        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }

    // ===== NEW: ARM ANIMATION METHODS =====
    void HandleProceduralAnimation()
    {
        if (!leftArm && !rightArm) return; // Skip if no arms assigned
        
        // Idle breathing animation
        breathTimer += Time.deltaTime * 2f;
        float breath = Mathf.Sin(breathTimer) * idleBreathAmount;
        
        if (leftArm) 
        {
            leftArm.localRotation = Quaternion.Euler(breath, 0, 0);
            PositionArmInView(leftArm, -0.3f);
        }
        
        if (rightArm) 
        {
            rightArm.localRotation = Quaternion.Euler(breath, 0, 0);
            PositionArmInView(rightArm, 0.3f);
        }
        
        // Walking bob animation
        if (isGrounded && controller.velocity.magnitude > 0.5f)
        {
            walkBobTimer += Time.deltaTime * walkBobAmount;
            float bob = Mathf.Sin(walkBobTimer) * 15f;
            
            if (rightArm) rightArm.Rotate(0, 0, bob);
            if (leftArm) leftArm.Rotate(0, 0, -bob);
        }
        
        // Mouse sway animation
        float swayX = inputLookX * armSwayAmount;
        float swayY = inputLookY * armSwayAmount;
        
        if (leftArm) leftArm.Rotate(0, swayY, swayX);
        if (rightArm) rightArm.Rotate(0, swayY, -swayX);
    }
    
    void PositionArmInView(Transform arm, float xOffset)
    {
        // Keep arms positioned relative to camera
        arm.localPosition = new Vector3(xOffset, -0.4f, 0.5f);
    }
}