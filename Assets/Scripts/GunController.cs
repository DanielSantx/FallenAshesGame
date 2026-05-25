using UnityEngine;

// ============================================================
// GunController: Control del arma que orbita alrededor del
// jugador. Sigue el ratón para apuntar y dispara proyectiles
// con clic izquierdo, respetando una cadencia de fuego.
// Aplica las mejoras de daño y cadencia desde GameState.
// ============================================================
public class GunController : MonoBehaviour
{
    // Prefab de la bala, punto de spawn, cadencia y velocidad
    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.25f;
    public float bulletSpeed = 12f;
    public AudioClip shootSound; // Sonido al disparar

    public float orbitRadius = 0.6f; // Distancia orbital (no se usa en código, referencia visual)

    private float nextFireTime = 0f;      // Timestamp del próximo disparo permitido
    private SpriteRenderer spriteRenderer; // Para voltear el sprite del arma

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // Calcula la dirección hacia la posición del ratón en el mundo
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rota el arma para que apunte hacia el ratón
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Voltea el sprite si el arma apunta hacia la izquierda
        if (spriteRenderer != null)
            spriteRenderer.flipY = angle > 90 || angle < -90;

        // Cadencia actual (afectada por la mejora)
        float currentFireRate = fireRate;
        if (GameState.Instance != null)
            currentFireRate = fireRate * (1f - GameState.Instance.fireRateLevel * 0.05f);

        // Dispara si se mantiene el clic izquierdo y ha pasado la cadencia
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + currentFireRate;
            Shoot(direction);
        }
    }

    // Instancia la bala en el firePoint y la lanza en la dirección apuntada
    void Shoot(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;

        // Aplica el daño mejorado a la bala
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null && GameState.Instance != null)
        {
            float bonusDamage = GameState.Instance.damageLevel * 0.5f;
            bulletScript.damage = Mathf.RoundToInt(1 + bonusDamage);
        }

        // Reproduce el sonido de disparo
        if (AudioManager.Instance != null && shootSound != null)
            AudioManager.Instance.PlaySFX(shootSound);
    }
}
