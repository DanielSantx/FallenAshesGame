using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.25f;
    public float bulletSpeed = 12f;

    public float orbitRadius = 0.6f;

    private float nextFireTime = 0f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (spriteRenderer != null)
            spriteRenderer.flipY = angle > 90 || angle < -90;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot(direction);
        }
    }

    void Shoot(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;
    }
}
