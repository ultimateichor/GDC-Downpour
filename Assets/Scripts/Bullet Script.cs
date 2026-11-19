using UnityEngine;
using UnityEngine.Pool;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 2f;

    private IObjectPool<GameObject> bulletPool;
    private Rigidbody2D rb;
    private float lifeTimer;
    private bool isReleased;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        isReleased = false;
        lifeTimer = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void SetPool(IObjectPool<GameObject> pool)
    {
        bulletPool = pool;
    }

    private void Update()
    {
        transform.Translate(Vector2.right * bulletSpeed * Time.deltaTime);

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= bulletLifetime)
        {
            ReturnToPool();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (isReleased) return;
        isReleased = true;

        if (bulletPool != null)
        {
            bulletPool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBulletSpeed(float newSpeed)
    {
        bulletSpeed = newSpeed;
    }
}
