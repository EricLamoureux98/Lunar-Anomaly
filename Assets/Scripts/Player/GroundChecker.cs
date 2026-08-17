using System;
using UnityEngine;

namespace LunarAnomaly.Player
{
    public class GroundChecker : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] float coyoteTime;
        float coyoteTimer;

        [Header("Ground Check")]
        [SerializeField] Transform groundCheckPos;
        [SerializeField] float groundCheckRadius;
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] LayerMask whatIsMetal;
        LayerMask combinedMask;
        public bool IsGrounded; // { get; private set; }
        
        bool footstepInterior;
        public bool FootstepInterior => footstepInterior;

        [Header("Slope Handling")]
        [SerializeField] float maxSlopeAngle = 40f;
        [SerializeField] float minSlopeAngle = 10;
        public bool IsOnSlope; // {get; private set;}
        [HideInInspector] public Vector3 SlopeNormal { get; private set;}
        RaycastHit slopeHit;

        void Update()
        {
            CheckGround();

            if (IsGrounded)
            {
                CheckSlope();
            }
            else
            {
                IsOnSlope = false;
                SlopeNormal = Vector3.up;
            }
        }

        void Start()
        {
            combinedMask = whatIsGround | whatIsMetal;
        }

        void CheckGround()
        {
            IsGrounded = Physics.CheckSphere(groundCheckPos.position, groundCheckRadius, combinedMask);
        }

        void CheckSlope()
        {
            if (!IsGrounded)
            {
                IsOnSlope = false;
                SlopeNormal = Vector3.up;
                return;
            }

            if (Physics.Raycast(groundCheckPos.position, Vector3.down, out slopeHit, 0.6f, combinedMask))
            {
                if ((whatIsGround.value & (1 << slopeHit.collider.gameObject.layer)) != 0)
                {
                    footstepInterior = false;
                    //Debug.Log("Standing on lunar surface");
                }
                else if ((whatIsMetal.value & (1 << slopeHit.collider.gameObject.layer)) != 0)
                {
                    footstepInterior = true;
                    //Debug.Log("Standing in habitat");
                }

                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

                if (angle > minSlopeAngle && angle <= maxSlopeAngle)
                {
                    IsOnSlope = true;
                    SlopeNormal = slopeHit.normal;
                }
                else
                {
                    IsOnSlope = false;
                    SlopeNormal = Vector3.up;
                }
            }
        }

        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            // For walking up slopes
            Vector3 dir = direction.normalized;
            return Vector3.ProjectOnPlane(dir, slopeHit.normal);
        }

        public bool IsStandingOnSlope()
        {     
            return IsGrounded && IsOnSlope;
        }

        public bool CoyoteReady()
        {
            return coyoteTimer < coyoteTime;
        }

        // void OnDrawGizmosSelected()
        // {
        //     // Draw the raycast as a line
        //     Gizmos.color = Color.red;
        //     float rayLength = playerHeight * 0.5f + 0.3f;
        //     Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayLength);

        //     // draw a sphere at the end of the ray to show the hit area
        //     Gizmos.DrawWireSphere(transform.position + Vector3.down * rayLength, 0.05f);
        // }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPos.position, groundCheckRadius);
        }
    }
}