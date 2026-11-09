using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private bool isGrounded;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Player is grounded.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Player is no longer grounded.");
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
