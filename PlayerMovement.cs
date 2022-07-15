using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    private float moveSpeed = 6f;
    public float walkSpeed;
    public float sprintSpeed;

    float horizontalInput;
    float verticalInput;

    [Header("Jumping")]
    public float gravity = -9.81f;
    public float jumpHeight = 1f;

    [Header("Keybinds")]
    KeyCode jumpKey = KeyCode.Space;
    KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.01f;
    public LayerMask groundMask;
    bool isGrounded;

    Vector3 velocity;

    public MovementState state;

    // Update is called once per frame
    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        MyInput();
        MovePlayer();
        StateHandler();
    }

    public enum MovementState
    {
        walking,
        sprinting,
    }

    private void StateHandler()
    {
        // Mode - Sprinting
        if (isGrounded && Input.GetKey(sprintKey))
        {
            // Issue: can't sprint and jump
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (velocity.x == 0 && velocity.z == 0)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (isGrounded && Input.GetKey(jumpKey))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;

        controller.Move(move * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
