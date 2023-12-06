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

    public float crouchHeight;
    public float crouchSpeed;

    public float slideMinSpeed;
    public float slideMagnitude;
    public float slideDrag;

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
        Sliding
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Capsule = GetComponent<CapsuleCollider>();

        readyToJump = true;

        CurrentMovementMode = MovementMode.Walking;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        SpeedControl();
        MyInput();

        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (CurrentMovementMode == MovementMode.Falling)
        {
            if (grounded)
            {
                CurrentMovementMode = MovementMode.Walking;

                NormalHeight();

                return;
            }
        }

        if (CurrentMovementMode != MovementMode.Falling)
        {
            if (!grounded)
            {
                CurrentMovementMode = MovementMode.Falling;

                NormalHeight();

                return;
            }
        }

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);

            return;
        }

        if (Input.GetKey(crouchKey) && grounded)
        {
            if (rb.velocity.magnitude >= slideMinSpeed)
            {
                if (CurrentMovementMode != MovementMode.Sliding && CurrentMovementMode != MovementMode.Crouching)
                {
                    EnterSlide();

                    CrouchHeight();

                    CurrentMovementMode = MovementMode.Sliding;
                }
            }

            else
            {
                CrouchHeight();                

                CurrentMovementMode = MovementMode.Crouching;
            }

            return;
        }

        if (grounded)
        {
            CurrentMovementMode = MovementMode.Walking;

            NormalHeight();
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (CurrentMovementMode == MovementMode.Walking)
        {
            rb.drag = groundDrag;

            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            return;
        }

        if (CurrentMovementMode == MovementMode.Falling)
        {
            rb.drag = airDrag;

            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

            return;
        }

        if (CurrentMovementMode == MovementMode.Sliding)
        {
            rb.drag = slideDrag;

            return;
        }

        if(CurrentMovementMode == MovementMode.Crouching)
        {
            rb.drag = groundDrag;

            rb.AddForce(moveDirection.normalized * crouchSpeed * 10f, ForceMode.Force);

            return;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (CurrentMovementMode == MovementMode.Walking || CurrentMovementMode == MovementMode.Falling)
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

    private void EnterSlide()
    {
        rb.velocity = rb.velocity * slideMagnitude;
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