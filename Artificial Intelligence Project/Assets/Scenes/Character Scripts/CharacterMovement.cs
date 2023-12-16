using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterMovement : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina;
    public float staminaRegenCooldown;
    public float staminaRegenRate;

    private float Stamina;
    private float staminaLastDrainTime;

    [Header("Movement")]
    public float moveSpeed;
    public float sprintMoveSpeed;
    private bool Sprinting;
    public float sprintStaminaCost;

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
    public float wallCheckDistance;
    public float wallRunCooldown;
    public float wallrunJumpForce;
    public float wallrunInitialYVelocity;
    public float wallrunGravity;

    private bool CanWallRun;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    private bool upwardsRunning;
    private bool downwardsRunning;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;

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

        Stamina = maxStamina;

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
            if (CanWallRun)
            {
                CheckForWall();

                if (wallLeft || wallRight)
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

            Jump();

            EnterFall();

            Invoke(nameof(ResetJump), jumpCooldown);

            return;
        }

        if (CurrentMovementMode == MovementMode.WallRunning)
        {
            CheckForWall();

            if (!wallLeft && !wallRight)
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

        if (CurrentMovementMode == MovementMode.Walking)
        {
            if (Input.GetKey(sprintKey) && Stamina >= 10)
            {
                Sprinting = true;

                return;
            }

            if (!Input.GetKey(sprintKey) || Stamina <= 1)
            {
                Sprinting = false;

                return;
            }
        }

        if (CurrentMovementMode != MovementMode.Walking && grounded)
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

        if(CurrentMovementMode != MovementMode.Walking)
        {
            Sprinting = false;
        }

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        switch (CurrentMovementMode)
        {
            case MovementMode.Walking:
                if(Sprinting)
                {
                    rb.AddForce(moveDirection.normalized * sprintMoveSpeed * 10f, ForceMode.Force);

                    break;
                }

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

        if(Sprinting)
        {
            DrainStamina(sprintStaminaCost / 60);
        }

        if(Time.time - staminaLastDrainTime >= staminaRegenCooldown)
        {
            Stamina += staminaRegenRate;

            if(Stamina < 0)
            {
                Stamina = 0;
            }

            if(Stamina > maxStamina)
            {
                Stamina = maxStamina;
            }
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
            if(Sprinting)
            {
                if (flatVel.magnitude > moveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * sprintMoveSpeed;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }

                return;
            }

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

            return;
        }

        if (CurrentMovementMode == MovementMode.Crouching)
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
        if(CurrentMovementMode == MovementMode.WallRunning)
        {
            Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.Normalize((transform.up + wallNormal)) * wallrunJumpForce, ForceMode.Impulse);

            CanWallRun = false;

            Invoke(nameof(ResetWallRun), wallRunCooldown);

            return;
        }

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
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - wallrunGravity, rb.velocity.z);

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

        //float YVelocity = Mathf.Clamp(rb.velocity.y + wallrunInitialYVelocity, -wallrunInitialYVelocity, wallrunInitialYVelocity);

        rb.velocity = new Vector3(rb.velocity.x, wallrunInitialYVelocity, rb.velocity.z);

        rb.drag = wallrunDrag;

        CurrentMovementMode = MovementMode.WallRunning;
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private void ResetWallRun()
    {
        CanWallRun = true;
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

    private void DrainStamina(float DrainAmount)
    {
        Stamina -= DrainAmount;

        staminaLastDrainTime = Time.time;
    }
}