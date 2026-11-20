using UnityEngine;

public class LedgeCheck : MonoBehaviour
{
    [SerializeField] private BoxCollider2D wallCollider;
    [SerializeField] private CircleCollider2D ledgeCollider;
    [SerializeField] private Vector2 boxColliderRightOffset = new Vector2(0.5f, 0.43f);
    [SerializeField] private Vector2 boxColliderLeftOffset = new Vector2(-0.5f, 0.43f);
    [SerializeField] private Vector2 circleColliderRightOffset = new Vector2(0.55f, 0.325f);
    [SerializeField] private Vector2 circleColliderLeftOffset = new Vector2(-0.55f, 0.325f);

    private PlayerMovement playerMovement;
    private bool canGrab;

    private void Start()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponentInParent<PlayerMovement>();
        }
    }

    private void Update()
    {
        if (playerMovement.GetLookDirection() == LookDirection.Right || playerMovement.GetLookDirection() == LookDirection.UpRight || playerMovement.GetLookDirection() == LookDirection.DownRight)
        {
            wallCollider.offset = boxColliderRightOffset;
            ledgeCollider.offset = circleColliderRightOffset;
        }
        else if (playerMovement.GetLookDirection() == LookDirection.Left || playerMovement.GetLookDirection() == LookDirection.UpLeft || playerMovement.GetLookDirection() == LookDirection.DownLeft)
        {
            wallCollider.offset = boxColliderLeftOffset;
            ledgeCollider.offset = circleColliderLeftOffset;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

    public void SetBoxColliderOffset(Vector2 newOffset)
    {
        wallCollider.offset = newOffset;
    }

    public void SetCircleColliderOffset(Vector2 newOffset)
    {
        ledgeCollider.offset = newOffset;
    }
}
