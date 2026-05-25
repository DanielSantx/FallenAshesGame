using System.Collections;
using UnityEngine;

// ============================================================
// PortalGuardTrigger: Detecta cuándo el jugador se acerca al
// portal de la mazmorra. Si no ha hablado con el rey, los
// guardias se ponen en alerta (cambian sprite) y activan un
// muro invisible. Si ya ha hablado, le dejan pasar.
// ============================================================
public class PortalGuardTrigger : MonoBehaviour
{
    // Referencias a los sprites de los dos guardias
    [Header("Guardias")]
    public SpriteRenderer guard1Renderer;
    public SpriteRenderer guard2Renderer;

    // Sprites en estado de reposo (tranquilos)
    [Header("Sprites Reposo")]
    public Sprite guard1Normal;
    public Sprite guard2Normal;

    // Sprites en estado de alerta (bloquean el paso)
    [Header("Sprites Alerta")]
    public Sprite guard1Alert;
    public Sprite guard2Alert;

    // Collider del muro invisible que bloquea el paso al portal
    [Header("Muro invisible")]
    public Collider2D wallCollider;

    // Diálogos antes y después de hablar con el rey
    [Header("Diálogos")]
    [TextArea(2, 6)]
    public string[] dialogueBeforeKing;
    [TextArea(2, 6)]
    public string[] dialogueAfterKing;

    // Flags para mostrar el diálogo solo la primera vez
    private bool firstTimeBefore = true;
    private bool firstTimeAfter = true;

    void Start()
    {
        wallCollider.enabled = false;
        SetGuardsNormal();
    }

    // Al entrar el jugador en el trigger del portal
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Si NO ha hablado con el rey → guardias alerta + muro
        if (!GameState.Instance.hasSpokenToKing)
        {
            SetGuardsAlert();
            wallCollider.enabled = true;

            // Diálogo de bloqueo solo la primera vez
            if (firstTimeBefore)
            {
                firstTimeBefore = false;
                StartCoroutine(ShowDialogueDelayed(dialogueBeforeKing, null));
            }
        }
        else
        {
            // Si ya habló con el rey → guardias tranquilos, sin muro
            if (firstTimeAfter)
            {
                firstTimeAfter = false;
                SetGuardsNormal();
                wallCollider.enabled = false;
                StartCoroutine(ShowDialogueDelayed(dialogueAfterKing, null));
            }
            else
            {
                // Ya pasó antes, simplemente le dejan pasar
                SetGuardsNormal();
                wallCollider.enabled = false;
            }
        }
    }

    // Muestra el diálogo con un pequeño retardo
    IEnumerator ShowDialogueDelayed(string[] lines, System.Action callback)
    {
        yield return new WaitForSeconds(0.1f);
        DialogueManager.Instance.StartDialogue("Guardia", lines, callback);
    }

    // Cambia los sprites de los guardias a estado alerta
    void SetGuardsAlert()
    {
        if (guard1Renderer) guard1Renderer.sprite = guard1Alert;
        if (guard2Renderer) guard2Renderer.sprite = guard2Alert;
    }

    // Cambia los sprites de los guardias a estado normal
    void SetGuardsNormal()
    {
        if (guard1Renderer) guard1Renderer.sprite = guard1Normal;
        if (guard2Renderer) guard2Renderer.sprite = guard2Normal;
    }
}