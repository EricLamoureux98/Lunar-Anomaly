using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;
    GroundChecker groundChecker;
    Rigidbody rb;

    [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag = 0.05f;
    [SerializeField] float airControlSpeed;
    [SerializeField] float jumpCooldown;
    Vector3 moveDirection;
    Vector2 moveInput;
    float currentSpeed;
    
    bool exitingSlope;
    bool readyToJump;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundChecker = GetComponent<GroundChecker>();
    }

    void Start()
    {
        currentSpeed = walkSpeed;
        readyToJump = true;
    }

    void FixedUpdate()
    {
        //HandleGravity();
        MovePlayer();
        HandleDrag();
    }

    void MovePlayer()
    {
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        orientation.forward = camForward.normalized;
        orientation.right = camRight.normalized;

        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x; 
        
        if (groundChecker.IsStandingOnSlope() && !exitingSlope)
        {
            HandleSlopeMovement();
        }
        else if (groundChecker.IsGrounded)
        {
            HandleGroundMovement();
        }
        else if (!groundChecker.IsGrounded)
        {
            HandleAirMovement();
        }
    }

    void ApplyJump()
    {
        exitingSlope = true;

        // Reset y velocity - Makes jump height consistent
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Directly setting velocity – overrides physics

        rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    void ApplyJumpInput()
    {
        if (readyToJump && groundChecker.CoyoteReady() && groundChecker.IsGrounded)
        {
            readyToJump = false;
            ApplyJump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }           
    }

    // Not working right I think!
    void HandleGravity()
    {
        if (groundChecker.IsStandingOnSlope() && !exitingSlope)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }
    }

    void HandleDrag()
    {
        if(groundChecker.IsGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
        }
    }

    void HandleGroundMovement()
    {
        rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
    }

    void HandleSlopeMovement()
    {
        
        Vector3 slopeDir = groundChecker.GetSlopeMoveDirection(moveDirection);

        rb.AddForce(slopeDir * currentSpeed * 10, ForceMode.Force);

        // Stick to slope
        if (rb.linearVelocity.y > 0)
        {
            rb.AddForce(Vector3.down * 10f, ForceMode.Force);
        }
    }

    void HandleAirMovement()
    {
        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Prevents constant acceleration
        if (horizontalVel.magnitude < currentSpeed)
        {
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airControlSpeed, ForceMode.Force);
        }        
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed) currentSpeed = sprintSpeed;

        if (context.canceled) currentSpeed = walkSpeed;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed) ApplyJumpInput();
    }

}
