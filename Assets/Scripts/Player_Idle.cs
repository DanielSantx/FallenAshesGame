using UnityEngine;

public class Player_Idle: MonoBehaviour
{
    public static bool inputBlocked = false;

    public AudioClip footstepSound;
    public float footstepInterval = 0.4f;

    float speed = 4;
    
    Rigidbody2D rb2D;

    Vector2 movementInput;
    private float nextFootstepTime = 0f;
    private AudioSource footstepSource;

    Animator animator;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.loop = true;
        footstepSource.playOnAwake = false;
        if (footstepSound != null) footstepSource.clip = footstepSound;
    }

    void Update()
    {
        // Si hay dialogo activo o cinemática, no procesar input
        if (inputBlocked || (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel != null && DialogueManager.Instance.dialoguePanel.activeSelf))
        {
            movementInput = Vector2.zero;
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            return;
        }

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        movementInput.Normalize();

        animator.SetFloat("Horizontal", (movementInput.x));
        animator.SetFloat("Vertical", (movementInput.y));

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
            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        rb2D.linearVelocity = movementInput * speed;
    }
}
