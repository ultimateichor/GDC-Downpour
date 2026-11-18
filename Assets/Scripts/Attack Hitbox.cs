using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private bool hitSomething;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hitSomething)
        {
            hitSomething = true;
            if (collision.CompareTag("Enemy"))
            {
                Debug.Log("Hit an enemy!");
                // Add logic for damaging the enemy here
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hitSomething)
        {
            hitSomething = false;
        }
    }

    public bool IsHittingSomething()
    {
        return hitSomething;
    }

    public void ChangeHitbox(Vector2 newSize, Vector2 newOffset)
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.size = newSize;
            boxCollider.offset = newOffset;
        }
    }
}
