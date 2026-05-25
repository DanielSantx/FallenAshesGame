using UnityEngine;

// ============================================================
// Enemy: IA básica para enemigos. Detecta al jugador en un
// rango, lo persigue, ataca con cooldown, recibe daño y muere.
// Dispara el evento OnDeath para que RoomManager controle las
// oleadas.
// ============================================================
public class Enemy : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Movimiento")]
    public float detectionRange = 6f;   // Distancia a la que detecta al jugador
    public float attackRange = 1.5f;    // Distancia a la que ataca
    public float moveSpeed = 2f;        // Velocidad de persecución

    [Header("Ataque")]
    public float attackDamage = 0.5f;   // Daño por golpe (medio corazón)
    public float attackCooldown = 1f;   // Segundos entre ataques
    private float nextAttackTime = 0f;  // Timestamp del próximo ataque

    [Header("Sonido")]
    public AudioClip hurtSound;         // Sonido al recibir daño

    [Header("Almas")]
    public GameObject SoulPrefab;       // Prefab del alma que suelta al morir
    public int soulDropAmount = 1;      // Cantidad de almas por muerte

    [Header("Referencias")]
    public Transform player;            // Se auto-asigna en Start si está vacío
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Evento que se dispara al morir (lo escucha RoomManager)
    public System.Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Si la referencia al jugador está vacía, la busca automáticamente
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Si el jugador está dentro del rango de detección
        if (distance <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Persigue si está lejos
            if (distance > attackRange)
            {
                rb.linearVelocity = direction * moveSpeed;
                if (animator != null) animator.SetFloat("Speed", rb.linearVelocity.magnitude);
            }
            else
            {
                // Ataca si está cerca y ha pasado el cooldown
                rb.linearVelocity = Vector2.zero;
                if (animator != null) animator.SetFloat("Speed", 0);

                if (Time.time >= nextAttackTime)
                {
                    nextAttackTime = Time.time + attackCooldown;
                    if (animator != null) animator.SetTrigger("Attack");
                    Attack(direction);
                }
            }

            // Voltea el sprite según la dirección
            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x < 0;
        }
        else
        {
            // Fuera de rango: se queda quieto
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetFloat("Speed", 0);
        }
    }

    // Aplica el daño al jugador si está en rango
    void Attack(Vector2 direction)
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(attackDamage);
        }
    }

    // Recibe daño desde Bullet.cs, reproduce sonido y comprueba muerte
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (AudioManager.Instance != null && hurtSound != null)
            AudioManager.Instance.PlaySFX(hurtSound);
        if (currentHealth <= 0)
            Die();
        else if (animator != null)
            animator.SetTrigger("Hurt");
    }

    // Animación de muerte, notifica a RoomManager y se destruye
    void Die()
    {
        // Suelta almas al morir
        if (SoulPrefab != null)
        {
            GameObject soul = Instantiate(SoulPrefab, transform.position, Quaternion.identity);
            Soul soulScript = soul.GetComponent<Soul>();
            if (soulScript != null) soulScript.value = soulDropAmount;
        }

        if (animator != null) animator.SetTrigger("Die");
        if (OnDeath != null) OnDeath();
        Destroy(gameObject, 0.3f);
    }

    // Dibuja los rangos de detección y ataque en el Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
