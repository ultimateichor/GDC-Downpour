using UnityEngine;
using UnityEngine.Pool;

public class PlayerBulletSpawnpoint : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int defaultPoolCapacity = 5;
    [SerializeField] private int maxPoolCapacity = 10;

    private IObjectPool<GameObject> bulletPool;

    private void Awake()
    {
        bulletPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: defaultPoolCapacity,
            maxSize: maxPoolCapacity
            );
    }

    public void SpawnBullet(float bulletSpeed)
    {
        GameObject bullet = bulletPool.Get();
        bullet.GetComponent<BulletScript>().SetBulletSpawn(transform.position, transform.rotation);
        bullet.GetComponent<BulletScript>().SetBulletSpeed(bulletSpeed);

        bullet.GetComponent<BulletScript>().SetPool(bulletPool);
    }

    public void ChangeSpawn(Vector2 newPosition, Quaternion newRotation)
    {
        transform.localPosition = newPosition;
        transform.localRotation = newRotation;
    }

    private void OnDrawGizmos2D()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, .5f);
    }
}
