using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHearts = 5;
    private float currentHealth;

    [Header("UI Corazones")]
    public GameObject heartContainer;
    public GameObject heartPrefab;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    private Image[] hearts;

    void Start()
    {
        currentHealth = maxHearts;
        SetupHearts();
    }

    void SetupHearts()
    {
        if (heartContainer == null || heartPrefab == null) return;

        hearts = new Image[maxHearts];
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject h = Instantiate(heartPrefab, heartContainer.transform);
            hearts[i] = h.GetComponent<Image>();
        }
        UpdateHearts();
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHearts();
    }

    public void HealFull()
    {
        currentHealth = maxHearts;
        UpdateHearts();
    }

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

    void Die()
    {
        Debug.Log("Player died");
    }
}
