using LunarAnomaly.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LunarAnomaly.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Transform cam;
        [SerializeField] Transform orientation;
        InputHandler inputHandler;
        GroundChecker groundChecker;
        Rigidbody rb;

        [Header("Movement")]
        [SerializeField] float walkSpeed;
        [SerializeField] float sprintSpeed;
        [SerializeField] float jumpHeight;
        [SerializeField] float groundDrag;
        [SerializeField] float airDrag = 0.05f;
        [SerializeField] float airControlSpeed;
        [SerializeField] float extraFallForce;
        [SerializeField] float jumpCooldown;
        [SerializeField] private float bumpThreshold = 2f;
        Vector3 moveDirection;
        Vector2 moveInput;
        float currentSpeed;

        [Header("Sound")]
        [SerializeField] float walkingSoundTime = 0.5f;
        [SerializeField] float runningSoundTime = 0.25f;
        float currentSoundTime;
        float soundTimer;
        
        bool exitingSlope;
        bool readyToJump;
        bool movementActive;
        bool isSprinting;
        bool jumpingHeld;
        bool wasJumpingHeldLastFrame;

        // Add variable jump height

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            groundChecker = GetComponent<GroundChecker>();
        }

        void Start()
        {
            movementActive = true;
            currentSpeed = walkSpeed;
            readyToJump = true;
        }

        void Update()
        {
            ReadInput();
            HandleInputs();
        }

        void FixedUpdate()
        {
            if (!movementActive) return; 

            ApplyExtraGravity();
            MovePlayer();
            HandleDrag();
            soundTimer += Time.fixedDeltaTime;
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
            wasJumpingHeldLastFrame = false;
        }

        void ApplyJumpInput()
        {
            if (readyToJump && groundChecker.CoyoteReady() && groundChecker.IsGrounded)
            {
                wasJumpingHeldLastFrame = true;
                readyToJump = false;
                ApplyJump();
                Invoke(nameof(ResetJump), jumpCooldown);
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
            if (soundTimer >= currentSoundTime && Mathf.Abs(rb.linearVelocity.x) > 0.5f)
            {
                SoundManager.PlaySound(SoundType.Footstep, 0.1f, false);
                soundTimer = 0f;
            }

            rb.useGravity = true;

            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
            rb.AddForce(Vector3.down * 3f, ForceMode.Force);
            
            if (groundChecker.IsGrounded && rb.linearVelocity.y > 0f && rb.linearVelocity.y < bumpThreshold)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            }
        }

        void HandleSlopeMovement()
        {        
            if (soundTimer >= currentSoundTime && Mathf.Abs(rb.linearVelocity.x) > 0.5f)
            {
                SoundManager.PlaySound(SoundType.Footstep, 0.1f, false);
                soundTimer = 0f;
            }

            rb.useGravity = false;

            Vector3 slopeDir = groundChecker.GetSlopeMoveDirection(moveDirection);

            // Prevent downhill acceleration
            //if (rb.linearVelocity.magnitude < currentSpeed)
                rb.AddForce(slopeDir.normalized * currentSpeed * 10, ForceMode.Force);

            rb.AddForce(-groundChecker.SlopeNormal * 10f, ForceMode.Force);

            // Prevent sliding when player not moving
            if (moveDirection == Vector3.zero)
                rb.linearVelocity = Vector3.zero;
        }

        void HandleAirMovement()
        {
            rb.useGravity = true;
            
            Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Prevents constant acceleration
            if (horizontalVel.magnitude < currentSpeed)
            {
                rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airControlSpeed, ForceMode.Force);
            }        
        }

        void ApplyExtraGravity()
        {
            // Stronger gravity while falling
            if (rb.linearVelocity.y < 0)
            {
                rb.AddForce(Vector3.down * extraFallForce, ForceMode.Force);           
            }
        }

        public void SetActive(bool active)
        {
            movementActive = active;
        }

        void ReadInput()
        {
            moveInput = inputHandler.MoveInput;
            isSprinting = inputHandler.SprintHeld;
            jumpingHeld = inputHandler.JumpHeld;
        }

        void HandleInputs()
        {
            if (isSprinting)
            {
                if (!groundChecker.IsGrounded) return;
                
                currentSpeed = sprintSpeed;
                currentSoundTime = runningSoundTime;
            }
            else
            {
                currentSpeed = walkSpeed;
                currentSoundTime = walkingSoundTime;
            }

            if (jumpingHeld && !wasJumpingHeldLastFrame)
            {
                ApplyJumpInput();
            }
        }
    }
}