using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// ============================================================
// CinematicManager: Cinemática inicial del juego. Se ejecuta
// al cargar el castillo tras el carrusel de Lore (solo partida
// nueva). El mago se acerca al jugador, le da un discurso de
// introducción y luego vuelve a su sitio. El jugador no puede
// moverse durante la cinemática.
// ============================================================
public class CinematicManager : MonoBehaviour
{
    [Header("References")]
    public Transform mageTransform;          // El NPC Mago
    public Transform playerTransform;        // El jugador
    public Vector3 mageOffsetFromPlayer = new Vector3(1.5f, 0, 0); // Dónde se coloca el mago al hablar

    [Header("Tiempos")]
    public float fadeDuration = 0.5f; // Duración del fundido inicial
    public float moveSpeed = 2f;      // Velocidad a la que se mueve el mago

    [Header("Dialogo introductorio")]
    [TextArea(2, 6)]
    public string[] introDialogueLines; // Líneas que dice el mago

    private Vector3 mageOriginalPos; // Posición original del mago (para volver)
    private Image fadeOverlay;       // Capa de fundido creada dinámicamente

    void Start()
    {
        // Solo se ejecuta si la bandera del carrusel de Lore está activa
        if (!LoreManager.flagPlayIntro)
        {
            enabled = false;
            return;
        }
        LoreManager.flagPlayIntro = false;
        StartCoroutine(PlayIntro());
    }

    // Secuencia completa de la cinemática
    IEnumerator PlayIntro()
    {
        // Bloquea el movimiento del jugador
        Player_Idle.inputBlocked = true;
        mageOriginalPos = mageTransform.position;

        // Desactiva componentes del mago que interferirían
        Animator mageAnimator = mageTransform.GetComponent<Animator>();
        NPC mageNpc = mageTransform.GetComponent<NPC>();
        if (mageNpc != null) mageNpc.enabled = false;
        if (mageAnimator != null) mageAnimator.enabled = false;

        // Fundido de entrada (negro → visible)
        CreateFadeOverlay();
        yield return Fade(1, 0);
        Destroy(fadeOverlay.transform.parent.gameObject);

        yield return new WaitForSeconds(0.5f);

        // El mago camina hacia el jugador
        Vector3 target = playerTransform.position + mageOffsetFromPlayer;
        yield return MoveMage(target, moveSpeed);

        // Reactiva el animator para el diálogo (el mago "mira" al jugador)
        if (mageAnimator != null) mageAnimator.enabled = true;

        // Diálogo introductorio (congela el tiempo mientras habla)
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue("Mage", introDialogueLines);
            yield return new WaitUntil(() => !DialogueManager.Instance.dialoguePanel.activeSelf);
        }

        if (mageAnimator != null) mageAnimator.enabled = false;

        // Pequeña pausa y el mago vuelve a su sitio
        yield return new WaitForSeconds(0.3f);
        yield return MoveMage(mageOriginalPos, moveSpeed * 1.5f);

        // Reactiva todo y libera al jugador
        if (mageAnimator != null) mageAnimator.enabled = true;
        if (mageNpc != null) mageNpc.enabled = true;
        Player_Idle.inputBlocked = false;
    }

    // Crea un Canvas con una imagen negra para el fundido
    void CreateFadeOverlay()
    {
        GameObject canvasGO = new GameObject("CinematicFadeCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        GameObject imgGO = new GameObject("FadeImage");
        imgGO.transform.SetParent(canvasGO.transform, false);
        fadeOverlay = imgGO.AddComponent<Image>();
        fadeOverlay.color = Color.black;
        fadeOverlay.raycastTarget = false;

        RectTransform rt = fadeOverlay.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    // Mueve un objeto hacia un objetivo con velocidad constante
    IEnumerator MoveMage(Vector3 target, float speed)
    {
        while (Vector3.Distance(mageTransform.position, target) > 0.05f)
        {
            mageTransform.position = Vector3.MoveTowards(
                mageTransform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    // Fundido del overlay: from → to en canal alpha
    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;
            Color c = fadeOverlay.color;
            c.a = Mathf.Lerp(from, to, t);
            fadeOverlay.color = c;
            yield return null;
        }
    }
}
