using UnityEngine;
using TMPro;
using System.Collections;

// ============================================================
// DialogueManager: Singleton que gestiona los diálogos con NPCs.
// Muestra un panel con el nombre del NPC, el texto con efecto
// de máquina de escribir, y un prompt para continuar. Congela
// el tiempo (timeScale = 0) mientras el diálogo está abierto.
// ============================================================
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public static bool justEnded = false; // Flag para evitar re-trigger en el mismo frame

    [Header("Sonido")]
    public AudioClip typingSound; // Sonido que se reproduce por cada carácter

    [Header("UI References")]
    public GameObject dialoguePanel;     // Panel del diálogo (con todos los hijos)
    public TMP_Text npcNameText;          // Nombre del NPC hablando
    public TMP_Text dialogueText;         // Texto del diálogo
    public GameObject continuePrompt;     // Indicador "pulsa E para continuar"

    private string[] currentLines;        // Líneas del diálogo actual
    private int currentIndex;             // Índice de la línea actual
    private bool isTyping;                // Si está escribiendo (efecto máquina)
    private bool inputBlocked = false;    // Bloqueo inicial para evitar skip instantáneo
    private System.Action onFinish;       // Callback al terminar el diálogo
    private Coroutine typingCoroutine;    // Referencia a la corrutina de typing

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        dialoguePanel.SetActive(false);
    }

    // Inicia un diálogo: muestra el panel, congela el tiempo y escribe línea a línea
    public void StartDialogue(string npcName, string[] lines, System.Action onComplete = null)
    {
        currentLines = lines;
        currentIndex = 0;
        onFinish = onComplete;
        npcNameText.text = npcName;
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;          // Congela el juego
        inputBlocked = true;
        StartCoroutine(UnblockInput());
        ShowLine();
    }

    // Pequeña pausa para evitar que la E del jugador avance el diálogo
    IEnumerator UnblockInput()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        inputBlocked = false;
    }

    // Muestra la línea actual con efecto de typing
    void ShowLine()
    {
        continuePrompt.SetActive(false);
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentIndex]));
    }

    // Escribe el texto carácter por carácter con sonido
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            if (AudioManager.Instance != null && typingSound != null)
                AudioManager.Instance.PlaySFX(typingSound);
            yield return new WaitForSecondsRealtime(0.03f);
        }
        isTyping = false;
        continuePrompt.SetActive(true); // Muestra "pulsa E para continuar"
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf || inputBlocked) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Si está escribiendo, completa la línea al instante
            if (isTyping)
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);
                isTyping = false;
                dialogueText.text = currentLines[currentIndex];
                continuePrompt.SetActive(true);
            }
            else
            {
                // Avanza a la siguiente línea o termina el diálogo
                currentIndex++;
                if (currentIndex < currentLines.Length)
                    ShowLine();
                else
                    EndDialogue();
            }
        }
    }

    // Cierra el panel, reanuda el tiempo y ejecuta el callback
    void EndDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        Time.timeScale = 1f;
        justEnded = true;
        onFinish?.Invoke();
    }

    // Resetea el flag justEnded al final del frame
    void LateUpdate()
    {
        justEnded = false;
    }
}
