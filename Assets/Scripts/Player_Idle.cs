using UnityEngine;

// ============================================================
// Player_Idle: Control de movimiento del jugador en 8 direcciones
// (WASD / flechas). Bloquea la entrada durante diálogos o
// cinemáticas. Gestiona el sonido de pasos.
// Aplica la mejora de velocidad desde GameState.
// ============================================================
public class Player_Idle: MonoBehaviour
{
    public static bool inputBlocked = false;

    // Clip de sonido de pasos (se reproduce en bucle mientras el jugador se mueve)
    public AudioClip footstepSound;
    public float footstepInterval = 0.4f;

    float speed = 4; // Velocidad base (se multiplica por mejora en Update)

    Rigidbody2D rb2D;

    Vector2 movementInput;
    private AudioSource footstepSource; // AudioSource dedicado para pasos

    Animator animator;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Crea un AudioSource para los pasos (bucle, se detiene al parar)
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.loop = true;
        footstepSource.playOnAwake = false;
        if (footstepSound != null) footstepSource.clip = footstepSound;
    }

    void Update()
    {
        // Bloquea input si hay cinemática activa o diálogo abierto
        if (inputBlocked || (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel != null && DialogueManager.Instance.dialoguePanel.activeSelf))
        {
            movementInput = Vector2.zero;
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            return;
        }

        // Lee entrada del teclado (eje horizontal y vertical)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        movementInput.Normalize(); // Evita que la diagonal duplique velocidad

        // Actualiza el Animator para la animación de movimiento
        animator.SetFloat("Horizontal", (movementInput.x));
        animator.SetFloat("Vertical", (movementInput.y));

        // Control del sonido de pasos: suena mientras hay movimiento
        if (movementInput != Vector2.zero)
        {
            if (footstepSource != null && footstepSound != null)
            {
                footstepSource.volume = AudioManager.Instance != null ? AudioManager.Instance.sfxVolume : 1f;
                if (!footstepSource.isPlaying) footstepSource.Play();
            }
        }
        else
        {
            // Se detiene al instante cuando el jugador se para
            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();
        }
    }

    // Aplica la velocidad calculada en Update al Rigidbody
    private void FixedUpdate()
    {
        float finalSpeed = speed;
        if (GameState.Instance != null)
            finalSpeed = speed * (1f + GameState.Instance.speedLevel * 0.2f);
        rb2D.linearVelocity = movementInput * finalSpeed;
    }
}
