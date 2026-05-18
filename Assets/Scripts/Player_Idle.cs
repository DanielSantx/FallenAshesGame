using UnityEngine;

public class Player_Idle: MonoBehaviour
{
    float speed = 4;
    
    Rigidbody2D rb2D;

    Vector2 movementInput;

    Animator animator;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Si hay dialogo activo, no procesar input
        if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel != null && DialogueManager.Instance.dialoguePanel.activeSelf)
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
    }

    private void FixedUpdate()
    {
        rb2D.linearVelocity = movementInput * speed;
    }
}
