using UnityEngine;
using System.Collections;

// ============================================================
// NPC: Interacción con personajes no jugadores. Detecta cuando
// el jugador está cerca (trigger Collider2D), muestra un prompt
// [E] y al pulsarlo inicia un diálogo mediante DialogueManager.
// Soporta diálogo condicional (antes/después de hablar con el
// rey) y callbacks para marcar eventos narrativos.
// ============================================================
public class NPC : MonoBehaviour
{
    [Header("Identificación")]
    public string npcName; // Nombre que aparece en el panel de diálogo

    [Header("Diálogo")]
    [TextArea(2, 6)]
    public string[] dialogueLines;       // Líneas de diálogo por defecto
    public bool hasConditionalDialogue = false; // ¿Tiene diálogo alternativo?
    [TextArea(2, 6)]
    public string[] dialogueLinesAfter; // Líneas después de cumplir condición

    [Header("Configuración")]
    public float interactionRange = 2f; // Distancia de interacción (debug visual)
    public string npcType;              // "king", "mage", etc. para callbacks específicos
    private bool playerInRange = false;
    public GameObject interactPrompt;   // Indicador [E] sobre el NPC

    void Start()
    {
        if (interactPrompt) interactPrompt.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt) interactPrompt.SetActive(true);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt) interactPrompt.SetActive(true);
        }
    }

    IEnumerator BlockInputBriefly()
    {
        yield return new WaitForSecondsRealtime(0.2f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt) interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // Mantiene el prompt [E] pegado al NPC en coordenadas de pantalla
        if (playerInRange && interactPrompt && interactPrompt.activeSelf)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(
                transform.position + new Vector3(0, 1f, 0)
            );
            interactPrompt.transform.position = screenPos;
        }

        // Al pulsar E si está en rango y no hay otro diálogo activo
        if (playerInRange && !DialogueManager.justEnded && Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance == null || DialogueManager.Instance.dialoguePanel == null || !DialogueManager.Instance.dialoguePanel.activeSelf)
            {
                Interact();
                StartCoroutine(BlockInputBriefly());
            }
        }
    }

    // Inicia el diálogo según el estado de la narrativa
    void Interact()
    {
        string[] linesToShow = dialogueLines;
        // Si tiene diálogo condicional y ya habló con el rey, usa el alternativo
        if (hasConditionalDialogue && GameState.Instance.hasSpokenToKing)
            linesToShow = dialogueLinesAfter;

        // Callback que se ejecuta al terminar el diálogo
        System.Action callback = null;
        if (npcType == "king")
            callback = () => { GameState.Instance.hasSpokenToKing = true; GameState.Instance.SaveGame(); };
        else if (npcType == "mage")
            callback = () => { GameState.Instance.hasSpokenToMage = true; GameState.Instance.SaveGame(); };

        DialogueManager.Instance.StartDialogue(npcName, linesToShow, callback);
    }

    // Dibuja el rango de interacción en el Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}