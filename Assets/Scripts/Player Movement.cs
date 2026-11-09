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
    [SerializeField] private float ledgeGrabDistanceX = 0.5f;
    [SerializeField] private float ledgeGrabDistanceY = 0.5f;

    private float currentMoveSpeed;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private GroundCheck groundCheck;
    private LedgeCheck ledgeCheck;
    private float coyoteTimeCounter;
    private bool facingRight = true;

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
        if (ledgeCheck == null)
        {
            ledgeCheck = GetComponentInChildren<LedgeCheck>();
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

        if (moveInput.x != 0)
        {
            if (Mathf.Sign(moveInput.x) == 1 && !facingRight)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z); // Flip the player sprite based on movement direction
                facingRight = true;
            }
            else if (Mathf.Sign(moveInput.x) == -1 && facingRight)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z); // Flip the player sprite based on movement direction
                facingRight = false;
            }
        }

        if (groundCheck.IsGrounded())
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time when grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease coyote time when not grounded
        }

        if (ledgeCheck.CanGrabLedge())
        {
            ClimbLedge();
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

    private void ClimbLedge()
    {
        Debug.Log("Climbing the ledge!");
        if (!facingRight)
        {
            transform.position = new Vector3(transform.position.x - ledgeGrabDistanceX, transform.position.y + ledgeGrabDistanceY, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + ledgeGrabDistanceX, transform.position.y + ledgeGrabDistanceY, transform.position.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.transform.position, groundCheck.GetComponent<Collider2D>().bounds.size);
        }
        if (ledgeCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(ledgeCheck.transform.position, ledgeCheck.GetComponent<BoxCollider2D>().bounds.size);
            Gizmos.DrawSphere(ledgeCheck.transform.position, ledgeCheck.GetComponent<CircleCollider2D>().radius);
        }
    }
}
