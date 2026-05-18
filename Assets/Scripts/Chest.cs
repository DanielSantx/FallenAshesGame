using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite closedSprite;
    public Sprite openSprite;
    public GameObject interactPrompt;

    private SpriteRenderer spriteRenderer;
    private bool isOpen = false;
    private bool playerInRange = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && closedSprite != null)
            spriteRenderer.sprite = closedSprite;
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && interactPrompt != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(
                transform.position + new Vector3(0, 1f, 0)
            );
            interactPrompt.transform.position = screenPos;
        }

        if (playerInRange && !isOpen && Input.GetKeyDown(KeyCode.E))
            Open();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Open()
    {
        isOpen = true;

        if (spriteRenderer != null && openSprite != null)
            spriteRenderer.sprite = openSprite;

        PlayerHealth ph = FindObjectOfType<PlayerHealth>();
        if (ph != null) ph.HealFull();

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}
