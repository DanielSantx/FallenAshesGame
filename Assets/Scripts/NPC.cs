using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    [Header("Identificaci�n")]
    public string npcName;
    [Header("Di�logo")]
    [TextArea(2, 6)]
    public string[] dialogueLines;
    public bool hasConditionalDialogue = false;
    [TextArea(2, 6)]
    public string[] dialogueLinesAfter;
    [Header("Configuraci�n")]
    public float interactionRange = 2f;
    public string npcType;
    private bool playerInRange = false;
    public GameObject interactPrompt;

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
        if (playerInRange && interactPrompt && interactPrompt.activeSelf)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(
                transform.position + new Vector3(0, 1f, 0)
            );
            interactPrompt.transform.position = screenPos;
        }

        if (playerInRange && !DialogueManager.justEnded && Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance == null || DialogueManager.Instance.dialoguePanel == null || !DialogueManager.Instance.dialoguePanel.activeSelf)
            {
                Interact();
                StartCoroutine(BlockInputBriefly());
            }
        }
    }

    void Interact()
    {
        string[] linesToShow = dialogueLines;
        if (hasConditionalDialogue && GameState.Instance.hasSpokenToKing)
            linesToShow = dialogueLinesAfter;
        System.Action callback = null;
        if (npcType == "king")
            callback = () => { GameState.Instance.hasSpokenToKing = true; GameState.Instance.SaveGame(); };
        else if (npcType == "mage")
            callback = () => { GameState.Instance.hasSpokenToMage = true; GameState.Instance.SaveGame(); };
        DialogueManager.Instance.StartDialogue(npcName, linesToShow, callback);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}