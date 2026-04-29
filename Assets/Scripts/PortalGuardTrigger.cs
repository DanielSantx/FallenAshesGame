using System.Collections;
using UnityEngine;

public class PortalGuardTrigger : MonoBehaviour
{
    [Header("Guardias")]
    public SpriteRenderer guard1Renderer;
    public SpriteRenderer guard2Renderer;

    [Header("Sprites Reposo")]
    public Sprite guard1Normal;  // NPCs 1_6
    public Sprite guard2Normal;  // NPCs 1_8

    [Header("Sprites Alerta")]
    public Sprite guard1Alert;   // NPCs 1_7
    public Sprite guard2Alert;   // NPCs 1_5

    [Header("Muro invisible")]
    public Collider2D wallCollider;

    [Header("Diálogos")]
    [TextArea(2, 6)]
    public string[] dialogueBeforeKing;
    [TextArea(2, 6)]
    public string[] dialogueAfterKing;

    private bool firstTimeBefore = true;
    private bool firstTimeAfter = true;
    private bool guardsOnAlert = false;

    void Start()
    {
        wallCollider.enabled = false;
        SetGuardsNormal();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!GameState.Instance.hasSpokenToKing)
        {
            // Guardia en alerta siempre
            SetGuardsAlert();
            wallCollider.enabled = true;

            // Dialogo solo la primera vez
            if (firstTimeBefore)
            {
                firstTimeBefore = false;
                StartCoroutine(ShowDialogueDelayed(dialogueBeforeKing, null));
            }
        }
        else
        {
            // Jugador habló con el Rey
            if (firstTimeAfter)
            {
                firstTimeAfter = false;
                SetGuardsNormal();
                wallCollider.enabled = false;
                StartCoroutine(ShowDialogueDelayed(dialogueAfterKing, null));
            }
            else
            {
                // Ya pasó antes, simplemente dejar pasar
                SetGuardsNormal();
                wallCollider.enabled = false;
            }
        }
    }

    IEnumerator ShowDialogueDelayed(string[] lines, System.Action callback)
    {
        yield return new WaitForSeconds(0.1f);
        DialogueManager.Instance.StartDialogue("Guardia", lines, callback);
    }

    void SetGuardsAlert()
    {
        if (guard1Renderer) guard1Renderer.sprite = guard1Alert;
        if (guard2Renderer) guard2Renderer.sprite = guard2Alert;
        guardsOnAlert = true;
    }

    void SetGuardsNormal()
    {
        if (guard1Renderer) guard1Renderer.sprite = guard1Normal;
        if (guard2Renderer) guard2Renderer.sprite = guard2Normal;
        guardsOnAlert = false;
    }
}