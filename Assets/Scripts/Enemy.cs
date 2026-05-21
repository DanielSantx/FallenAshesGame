using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Movimiento")]
    public float detectionRange = 6f;
    public float attackRange = 1.5f;
    public float moveSpeed = 2f;

    [Header("Ataque")]
    public float attackDamage = 0.5f;
    public float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    [Header("Sonido")]
    public AudioClip hurtSound;

    [Header("Referencias")]
    public Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public System.Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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

        if (distance <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (distance > attackRange)
            {
                rb.linearVelocity = direction * moveSpeed;
                if (animator != null) animator.SetFloat("Speed", rb.linearVelocity.magnitude);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                if (animator != null) animator.SetFloat("Speed", 0);

                if (Time.time >= nextAttackTime)
                {
                    nextAttackTime = Time.time + attackCooldown;
                    if (animator != null) animator.SetTrigger("Attack");
                    Attack(direction);
                }
            }

            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x < 0;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetFloat("Speed", 0);
        }
    }

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

    void Die()
    {
        if (animator != null) animator.SetTrigger("Die");
        if (OnDeath != null) OnDeath();
        Destroy(gameObject, 0.3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
