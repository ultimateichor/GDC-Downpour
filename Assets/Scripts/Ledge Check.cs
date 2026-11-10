using UnityEngine;

public class LedgeCheck : MonoBehaviour
{
    [SerializeField] private BoxCollider2D wallCollider;
    [SerializeField] private CircleCollider2D ledgeCollider;
    private bool canGrab;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && collision.IsTouching(wallCollider)) return;

        if (collision.CompareTag("Ground") && collision.IsTouching(ledgeCollider))
        {
            canGrab = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            canGrab = false;
        }
    }

    public bool CanGrabLedge()
    {
        return canGrab;
    }
}
