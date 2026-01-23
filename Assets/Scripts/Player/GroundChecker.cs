using System;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float coyoteTime;
    float coyoteTimer;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask whatIsGround;
    [HideInInspector] public bool IsGrounded { get; private set; }

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle = 40f;
    [HideInInspector] public bool IsOnSlope {get; private set;}
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

    void CheckGround()
    {
        // Maybe remove layermask later
        IsGrounded = Physics.CheckSphere(groundCheckPos.position, groundCheckRadius, whatIsGround);
    }

    void CheckSlope()
    {
        if (Physics.Raycast(groundCheckPos.position, Vector3.down, out slopeHit, groundCheckRadius + 0.2f, whatIsGround))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            if (angle > 0f && angle <= maxSlopeAngle)
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
