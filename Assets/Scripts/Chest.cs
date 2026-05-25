using UnityEngine;
using System.Collections;

// ============================================================
// Chest: Cofre interactivo. Al pulsar [E] cerca, cambia su
// sprite a abierto, cura al jugador al máximo, reproduce un
// sonido y muestra un efecto visual (+1UP) que flota hacia
// arriba y se desvanece.
// ============================================================
public class Chest : MonoBehaviour
{
    public Sprite closedSprite;   // Sprite del cofre cerrado
    public Sprite openSprite;     // Sprite del cofre abierto
    public GameObject interactPrompt; // Indicador [E] sobre el cofre
    public AudioClip openSound;   // Sonido al abrir

    public Sprite healSprite;            // Sprite del efecto de curación (+1UP)
    public float healFloatSpeed = 1.5f;  // Velocidad a la que flota hacia arriba
    public float healLifetime = 1f;      // Duración del efecto antes de desaparecer

    private SpriteRenderer spriteRenderer;
    private bool isOpen = false;
    private bool playerInRange = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Muestra el sprite cerrado al inicio
        if (spriteRenderer != null && closedSprite != null)
            spriteRenderer.sprite = closedSprite;
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        // Sigue al cofre en pantalla (para el prompt [E])
        if (playerInRange && interactPrompt != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(
                transform.position + new Vector3(0, 1f, 0)
            );
            interactPrompt.transform.position = screenPos;
        }

        // Abre al pulsar E si el jugador está cerca y no está ya abierto
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

    // Abre el cofre: cambia sprite, cura, suena y muestra efecto
    void Open()
    {
        isOpen = true;

        if (spriteRenderer != null && openSprite != null)
            spriteRenderer.sprite = openSprite;
        if (AudioManager.Instance != null && openSound != null)
            AudioManager.Instance.PlaySFX(openSound);

        // Cura al jugador a toda la vida
        PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
        if (ph != null) ph.HealFull();

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        // Efecto visual de curación (+1UP flotante)
        if (healSprite != null)
            StartCoroutine(SpawnHealEffect());
    }

    // Crea un sprite que flota hacia arriba y se desvanece
    IEnumerator SpawnHealEffect()
    {
        GameObject healGO = new GameObject("HealEffect");
        healGO.transform.position = transform.position + new Vector3(0, 1f, 0);
        SpriteRenderer sr = healGO.AddComponent<SpriteRenderer>();
        sr.sprite = healSprite;
        sr.sortingOrder = 10; // Encima de casi todo

        float elapsed = 0f;
        while (elapsed < healLifetime)
        {
            elapsed += Time.deltaTime;
            healGO.transform.position += Vector3.up * healFloatSpeed * Time.deltaTime;
            // Desvanece gradualmente
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, elapsed / healLifetime);
            sr.color = c;
            yield return null;
        }

        Destroy(healGO);
    }
}
