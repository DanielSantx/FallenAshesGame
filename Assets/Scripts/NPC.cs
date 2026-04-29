using UnityEngine;
using System.Collections;
public class NPC : MonoBehaviour
{
    [Header("Identificación")]
    public string npcName;
    [Header("Diálogo")]
    [TextArea(2, 6)]
    public string[] dialogueLines;
    public bool hasConditionalDialogue = false;
    [TextArea(2, 6)]
    public string[] dialogueLinesAfter;
    [Header("Configuración")]
    public float interactionRange = 2f;
    public string npcType;
    private bool playerInRange = false;
    public GameObject interactPrompt;

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
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (!DialogueManager.Instance.dialoguePanel.activeSelf)
                Interact();
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

    void Interact()
    {
        string[] linesToShow = dialogueLines;
        if (hasConditionalDialogue && GameState.Instance.hasSpokenToKing)
            linesToShow = dialogueLinesAfter;

        System.Action callback = null;
        if (npcType == "king")
            callback = () => GameState.Instance.hasSpokenToKing = true;
        else if (npcType == "mage")
            callback = () => GameState.Instance.hasSpokenToMage = true;

        DialogueManager.Instance.StartDialogue(npcName, linesToShow, callback);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}