using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public static bool justEnded = false;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text npcNameText;
    public TMP_Text dialogueText;
    public GameObject continuePrompt;

    private string[] currentLines;
    private int currentIndex;
    private bool isTyping;
    private bool inputBlocked = false;
    private System.Action onFinish;
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string npcName, string[] lines, System.Action onComplete = null)
    {
        currentLines = lines;
        currentIndex = 0;
        onFinish = onComplete;
        npcNameText.text = npcName;
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;
        inputBlocked = true;
        StartCoroutine(UnblockInput());
        ShowLine();
    }

    IEnumerator UnblockInput()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        inputBlocked = false;
    }

    void ShowLine()
    {
        continuePrompt.SetActive(false);
        // Solo para el typing, no StopAllCoroutines
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentIndex]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(0.03f);
        }
        isTyping = false;
        continuePrompt.SetActive(true);
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf || inputBlocked) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
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
                currentIndex++;
                if (currentIndex < currentLines.Length)
                    ShowLine();
                else
                    EndDialogue();
            }
        }
    }

    void EndDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        Time.timeScale = 1f;
        justEnded = true;
        onFinish?.Invoke();
    }

    void LateUpdate()
    {
        justEnded = false;
    }
}
