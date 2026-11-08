using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    private Vector2 moveInput;
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
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
            Debug.Log("Jump Released");
        }
    }

    private void OnCrouch(InputValue value)
    {
        if (value.isPressed)
        {
            Crouch();
        }
        else
        {
            // Implement logic for when crouch is released, if needed
            Debug.Log("Crouch Released");
        }
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        Debug.Log("Jump Pressed");
    }

    private void Crouch()
    {
        // Implement crouch logic here
        Debug.Log("Crouch pressed");
    }
}
