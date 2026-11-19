using UnityEngine;
using UnityEngine.InputSystem;

public enum LookDirection
{
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
}

public enum PlayerState
{
    Idle,
    Walking,
    Jumping,
    Falling,
    Crouching,
    Attacking,
    Shooting,
    Knockback,
    LedgeGrabbing,
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float maxMoveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float moveCutMultiplier = 0f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float knockbackDuration = 0.5f;
    [Header("Ledge Grab Settings")]
    [SerializeField] private float ledgeGrabDistanceX = 0.5f;
    [SerializeField] private float ledgeGrabDistanceY = 0.5f;
    [SerializeField] private float ledgeClimbDuration = 2f;

    private float currentMoveSpeed;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private GroundCheck groundCheck;
    private LedgeCheck ledgeCheck;
    private float coyoteTimeCounter;
    private bool isKnockedBack = false;
    private bool isLedgeGrabbing = false;
    private bool canMove = true;
    private bool wasMovingRight = true;
    private LookDirection lookDirection;
    private PlayerState currentState;

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
        SetLookDirection();
        if (moveInput == Vector2.zero && currentState == PlayerState.Walking)
        {
            SetPlayerState(PlayerState.Idle);
        }
        else if ((moveInput != Vector2.zero && groundCheck.IsGrounded()) || currentState == PlayerState.Idle)
        {
            SetPlayerState(PlayerState.Walking);
        }
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
        if (value.isPressed && groundCheck.IsGrounded())
        {
            SetPlayerState(PlayerState.Crouching); // Set player state to crouching
            Crouch(true);
        }
        else
        {
            SetPlayerState(PlayerState.Idle); // Reset player state to idle when not crouching
            Crouch(false);
        }
    }

    private void Update()
    {
        if (groundCheck.IsGrounded())
        {
            if (currentState == PlayerState.Falling)
            {
                SetPlayerState(PlayerState.Idle); // Reset player state to idle when grounded
            }
            coyoteTimeCounter = coyoteTime; // Reset coyote time when grounded
        }
        else
        {
            if (rb.linearVelocityY < 0 && currentState != PlayerState.Falling)
            {
                SetPlayerState(PlayerState.Falling); // Set player state to falling when not grounded
            }
            coyoteTimeCounter -= Time.deltaTime; // Decrease coyote time when not grounded
        }

        if ((ledgeCheck.CanGrabLedge()) && (moveInput.x != 0) && (!isLedgeGrabbing))
        {
            SetPlayerState(PlayerState.LedgeGrabbing);
            isLedgeGrabbing = true;
            ClimbLedge();
        }
    }

    private void FixedUpdate()
    {
        if (!isKnockedBack && canMove)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        if (moveInput.x == 0)
        {
            // Apply friction when no input is given
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * moveCutMultiplier, rb.linearVelocity.y);
            return;
        }
        float targetSpeed = moveInput.x * currentMoveSpeed;
        float speedDifference = targetSpeed - rb.linearVelocity.x;
        rb.AddForce(new Vector2(speedDifference * rb.mass * acceleration, 0), ForceMode2D.Force);
    }

    private void Jump()
    {
        if (groundCheck.IsGrounded() || coyoteTimeCounter > 0f)
        {
            SetPlayerState(PlayerState.Jumping); // Set player state to jumping
            rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
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
        StopPlayerMovement();
        if (lookDirection == LookDirection.Left || lookDirection == LookDirection.UpLeft || lookDirection == LookDirection.DownLeft)
        {
            transform.position = new Vector3(transform.position.x - ledgeGrabDistanceX, transform.position.y + ledgeGrabDistanceY, transform.position.z);
            Invoke("ResumePlayerMovement", ledgeClimbDuration);
            Invoke("StopLedgeGrabbing", ledgeClimbDuration);
        }
        else if (lookDirection == LookDirection.Right || lookDirection == LookDirection.UpRight || lookDirection == LookDirection.DownRight)
        {
            transform.position = new Vector3(transform.position.x + ledgeGrabDistanceX, transform.position.y + ledgeGrabDistanceY, transform.position.z);
            Invoke("ResumePlayerMovement", ledgeClimbDuration);
            Invoke("StopLedgeGrabbing", ledgeClimbDuration);
        }
    }

    private void StopLedgeGrabbing()
    {
        SetPlayerState(PlayerState.Idle); // Reset player state to idle after ledge climbing
        isLedgeGrabbing = false;
    }

    public void ApplyKnockback(float knockbackForce, Vector2 knockbackDirection)
    {
        if (isKnockedBack) return; // Prevent applying knockback if already in knockback state
        SetPlayerState(PlayerState.Knockback); // Set player state to knockback
        rb.linearVelocity = Vector2.zero; // Reset current velocity before applying knockback
        isKnockedBack = true;
        rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        Invoke("StopKnockback", knockbackDuration);
    }

    private void StopKnockback()
    {
        isKnockedBack = false;
        SetPlayerState(PlayerState.Idle); // Reset to idle state after knockback
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public void SetLookDirection()
    {
        if (moveInput == Vector2.zero)
        {
            if (wasMovingRight)
            {
                lookDirection = LookDirection.Right;
            }
            else
            {
                lookDirection = LookDirection.Left;
            }
            return;
        }

        switch (moveInput)
        {
            case Vector2 v when v.y > 0 && v.x == 0:
                if (lookDirection != LookDirection.Up)
                {
                    lookDirection = LookDirection.Up;
                }
                break;
            case Vector2 v when v.y < 0 && v.x == 0:
                if (lookDirection != LookDirection.Down)
                {
                    lookDirection = LookDirection.Down;
                }
                break;
            case Vector2 v when v.x < 0 && v.y == 0:
                if (lookDirection != LookDirection.Left)
                {
                    lookDirection = LookDirection.Left;
                    wasMovingRight = false;
                }
                break;
            case Vector2 v when v.x > 0 && v.y == 0:
                if (lookDirection != LookDirection.Right)
                {
                    lookDirection = LookDirection.Right;
                    wasMovingRight = true;
                }
                break;
            case Vector2 v when v.x < 0 && v.y > 0:
                if (lookDirection != LookDirection.UpLeft)
                {
                    lookDirection = LookDirection.UpLeft;
                }
                break;
            case Vector2 v when v.x > 0 && v.y > 0:
                if (lookDirection != LookDirection.UpRight)
                {
                    lookDirection = LookDirection.UpRight;
                }
                break;
            case Vector2 v when v.x < 0 && v.y < 0:
                if (lookDirection != LookDirection.DownLeft)
                {
                    lookDirection = LookDirection.DownLeft;
                }
                break;
            case Vector2 v when v.x > 0 && v.y < 0:
                if (lookDirection != LookDirection.DownRight)
                {
                    lookDirection = LookDirection.DownRight;
                }
                break;
        }
    }

    public void StopPlayerMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero; // Stop the player's movement immediately
    }

    public void ResumePlayerMovement()
    {
        canMove = true;
    }

    public LookDirection GetLookDirection()
    {
        return lookDirection;
    }

    public bool WasMovingRight()
    {
        return wasMovingRight;
    }

    public void SetPlayerState(PlayerState state)
    {
        currentState = state;
    }

    private void OnDrawGizmosSelected()
    {
        //if (groundCheck != null)
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawWireCube(groundCheck.GetComponent<Collider2D>().transform.position * groundCheck.GetComponent<Collider2D>().offset, groundCheck.GetComponent<Collider2D>().bounds.size);
        //}
        //if (ledgeCheck != null)
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawWireCube(ledgeCheck.GetComponent<BoxCollider2D>().transform.position * ledgeCheck.GetComponent<BoxCollider2D>().offset, ledgeCheck.GetComponent<BoxCollider2D>().bounds.size);
        //    Gizmos.DrawWireSphere(ledgeCheck.GetComponent<CircleCollider2D>().transform.position * ledgeCheck.GetComponent<CircleCollider2D>().offset, ledgeCheck.GetComponent<CircleCollider2D>().radius);
        //}
    }
}
