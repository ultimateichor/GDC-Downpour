using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float maxMoveSpeed = 5f;
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.2f;

    private float currentMoveSpeed;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private GroundCheck groundCheck;
    private float coyoteTimeCounter;

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (groundCheck == null)
        {
            groundCheck = GetComponentInChildren<GroundCheck>();
        }

        currentMoveSpeed = maxMoveSpeed;
        coyoteTimeCounter = coyoteTime;
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            Jump();
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
    }

    private void OnCrouch(InputValue value)
    {
        if (value.isPressed)
        {
            Crouch(true);
        }
        else
        {
            Crouch(false);
        }
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2(moveInput.x * currentMoveSpeed, rb.linearVelocity.y);

        if (groundCheck.IsGrounded())
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time when grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease coyote time when not grounded
        }
    }

    private void Jump()
    {
        if (groundCheck.IsGrounded() || coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            coyoteTimeCounter = 0f; // Reset coyote time after jumping
        }
    }

    private void Crouch(bool isCrouching)
    {
        if (isCrouching)
        {
            currentMoveSpeed *= crouchSpeedMultiplier;
        }
        else
        {
            currentMoveSpeed = maxMoveSpeed;
        }
    }
}
