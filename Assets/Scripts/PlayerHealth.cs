using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ============================================================
// PlayerHealth: Sistema de vida del jugador basado en corazones
// (5 corazones = 10 mitades de vida). Gestiona el daño recibido,
// la curación, la UI de corazones y la pantalla de muerte.
// ============================================================
public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHearts = 5;             // Número de corazones base (se suma mejora)
    private float currentHealth;          // Vida actual en unidades (1 corazón = 1.0)
    private float effectiveMaxHearts;     // maxHearts + mejora de vida (mitades)

    [Header("Sonido")]
    public AudioClip damageTakenSound;    // Sonido al recibir daño

    [Header("UI Corazones")]
    public GameObject heartContainer;     // Padre donde se instancian los corazones
    public GameObject heartPrefab;        // Prefab de cada corazón (con Image)
    public Sprite fullHeart;              // Corazón lleno
    public Sprite halfHeart;              // Corazón a medias
    public Sprite emptyHeart;             // Corazón vacío
    public GameObject deathScreen;        // Panel de muerte (HAS MUERTO)

    private Image[] hearts;               // Referencias a los Image de cada corazón

    void Start()
    {
        RefreshMaxHearts();
    }

    void SetupHearts()
    {
        if (heartContainer == null || heartPrefab == null) return;

        foreach (Transform child in heartContainer.transform)
            Destroy(child.gameObject);

        int heartCount = Mathf.CeilToInt(effectiveMaxHearts);
        hearts = new Image[heartCount];
        for (int i = 0; i < heartCount; i++)
        {
            GameObject h = Instantiate(heartPrefab, heartContainer.transform);
            hearts[i] = h.GetComponent<Image>();
        }
        UpdateHearts();
    }

    public void RefreshMaxHearts()
    {
        effectiveMaxHearts = maxHearts;
        if (GameState.Instance != null)
            effectiveMaxHearts = maxHearts + GameState.Instance.maxHpLevel * 0.5f;
        currentHealth = effectiveMaxHearts;
        SetupHearts();
    }

    // Aplica daño al jugador, reproduce sonido y comprueba muerte
    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        if (AudioManager.Instance != null && damageTakenSound != null)
            AudioManager.Instance.PlaySFX(damageTakenSound);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHearts();
    }

    // Cura al jugador a la vida máxima (usado por el cofre)
    public void HealFull()
    {
        currentHealth = effectiveMaxHearts;
        UpdateHearts();
    }

    // Refresca los sprites según la vida actual
    void UpdateHearts()
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            if (currentHealth >= i + 1)
                hearts[i].sprite = fullHeart;
            else if (currentHealth >= i + 0.5f)
                hearts[i].sprite = halfHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    // Muestra la pantalla de muerte y congela el tiempo
    void Die()
    {
        if (deathScreen != null)
            deathScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    // Botón de reaparición en la pantalla de muerte
    public void Respawn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Castle_MainScene");
    }
}
