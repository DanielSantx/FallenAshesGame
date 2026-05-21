using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
    public Sprite closedSprite;
    public Sprite openSprite;
    public GameObject interactPrompt;
    public AudioClip openSound;
    public Sprite healSprite;
    public float healFloatSpeed = 1.5f;
    public float healLifetime = 1f;

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
        if (AudioManager.Instance != null && openSound != null)
            AudioManager.Instance.PlaySFX(openSound);

        PlayerHealth ph = FindObjectOfType<PlayerHealth>();
        if (ph != null) ph.HealFull();

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (healSprite != null)
            StartCoroutine(SpawnHealEffect());
    }

    IEnumerator SpawnHealEffect()
    {
        GameObject healGO = new GameObject("HealEffect");
        healGO.transform.position = transform.position + new Vector3(0, 1f, 0);
        SpriteRenderer sr = healGO.AddComponent<SpriteRenderer>();
        sr.sprite = healSprite;
        sr.sortingOrder = 10;

        float elapsed = 0f;
        while (elapsed < healLifetime)
        {
            elapsed += Time.deltaTime;
            healGO.transform.position += Vector3.up * healFloatSpeed * Time.deltaTime;
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, elapsed / healLifetime);
            sr.color = c;
            yield return null;
        }

        Destroy(healGO);
    }
}
