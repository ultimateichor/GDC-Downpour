using UnityEngine;

public class LedgeCheck : MonoBehaviour
{
    [SerializeField] private BoxCollider2D wallCollider;
    [SerializeField] private CircleCollider2D ledgeCollider;
    private bool canGrab;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && collision.IsTouching(wallCollider)) return;

        if (collision.CompareTag("Ground") && collision.IsTouching(ledgeCollider))
        {
            canGrab = true;
            Debug.Log("Player can grab the ledge.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            canGrab = false;
            Debug.Log("Player can no longer grab the ledge.");
        }
    }

    public bool CanGrabLedge()
    {
        return canGrab;
    }
}
