using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 2f;
    [SerializeField] private Rigidbody2D rb;

    private void Start()
    {
        // Set the bullet's velocity based on its speed and direction
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * bulletSpeed;
        // Destroy the bullet after a certain time to prevent it from existing indefinitely
        Destroy(gameObject, bulletLifetime);
    }
}
