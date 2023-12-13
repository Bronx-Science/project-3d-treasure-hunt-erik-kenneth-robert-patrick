using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;
    public float airDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching/Sliding")]
    public float crouchHeight;
    public float crouchSpeed;

    public float slideMinSpeed;
    public float slideMagnitude;
    public float slideDrag;

    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public float wallrunDrag;
    public float wallRunForce;
    public float wallrunMaxSpeed;
    public float wallrunMinSpeed;
    public float wallCheckDistance;

    private bool CanWallRun;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Keybinds")]
    public KeyCode crouchKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    CapsuleCollider Capsule;

    MovementMode CurrentMovementMode;

    enum MovementMode
    {
        Walking,
        Falling,
        Crouching,
        Sliding,
        WallRunning
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Capsule = GetComponent<CapsuleCollider>();

        readyToJump = true;

        CurrentMovementMode = MovementMode.Walking;
    }
    private void FixedUpdate()
    {
        // ground check
        grounded = (Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround) && readyToJump);

        if(grounded)
        {
            CanWallRun = true;
        }

        if(!CanWallRun)
        {
            if (readyToJump)
            {
                CanWallRun = true;
            }
        }

        MyInput();
        SpeedControl();

        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (CurrentMovementMode == MovementMode.Falling)
        {
            if(CanWallRun)
            {
                CheckForWall();

                if (wallLeft || wallRight && Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) > wallrunMinSpeed)
                {
                    CanWallRun = false;

                    EnterWallRun();
                }
            }

            if (grounded)
            {
                EnterWalk();

                return;
            }
        }

        if (CurrentMovementMode != MovementMode.Falling && CurrentMovementMode != MovementMode.WallRunning)
        {
            if (!grounded)
            {
                EnterFall();

                return;
            }
        }

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && (grounded || CurrentMovementMode == MovementMode.WallRunning))
        {
            readyToJump = false;

            EnterFall();

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);

            return;
        }

        if(CurrentMovementMode == MovementMode.WallRunning)
        {
            CheckForWall();

            if ((!wallLeft && !wallRight) || rb.velocity.magnitude <= wallrunMinSpeed)
            {
                EnterFall();

                return;
            }
        }

        if (Input.GetKey(crouchKey) && grounded)
        {
            if (rb.velocity.magnitude >= slideMinSpeed)
            {
                if (CurrentMovementMode != MovementMode.Sliding && CurrentMovementMode != MovementMode.Crouching)
                {
                    EnterSlide();
                }
            }

            else
            {
                EnterCrouch();
            }

            return;
        }

        if (grounded)
        {
            EnterWalk();
        }
    }

    private void MovePlayer()
    {
        if(CurrentMovementMode != MovementMode.WallRunning && !rb.useGravity)
        {
            rb.useGravity = true;
        }

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        switch (CurrentMovementMode)
        {
            case MovementMode.Walking:
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

                break;

            case MovementMode.Falling:
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

                break;

            case MovementMode.Crouching:
                rb.AddForce(moveDirection.normalized * crouchSpeed * 10f, ForceMode.Force);

                break;

            case MovementMode.Sliding:
                break;

            case MovementMode.WallRunning:
                WallRun();

                break;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(CurrentMovementMode == MovementMode.WallRunning)
        {
            if (flatVel.magnitude > wallrunMaxSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

            return;
        }    

        if (CurrentMovementMode == MovementMode.Walking)
        {
            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

            return;
        }

        if(CurrentMovementMode == MovementMode.Crouching)
        {
            if(flatVel.magnitude > slideMinSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * (slideMinSpeed - 0.5f);
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

            return;
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private void WallRun()
    {
        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        // forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // push to wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    private void EnterWallRun()
    {
        rb.useGravity = false;

        rb.drag = wallrunDrag;

        CurrentMovementMode = MovementMode.WallRunning;
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }


    private void EnterWalk()
    {
        NormalHeight();

        rb.drag = groundDrag;

        CurrentMovementMode = MovementMode.Walking;
    }

    private void EnterFall()
    {
        NormalHeight();

        rb.drag = airDrag;

        CurrentMovementMode = MovementMode.Falling;
    }

    private void EnterCrouch()
    {
        CrouchHeight();

        rb.drag = groundDrag;

        CurrentMovementMode = MovementMode.Crouching;
    }

    private void EnterSlide()
    {
        CrouchHeight();

        rb.drag = slideDrag;

        rb.velocity = rb.velocity * slideMagnitude;

        CurrentMovementMode = MovementMode.Sliding;
    }

    private void NormalHeight()
    {
        Capsule.height = playerHeight;
    }

    private void CrouchHeight()
    {
        Capsule.height = crouchHeight;
    }
}