using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CinematicManager : MonoBehaviour
{
    [Header("References")]
    public Transform mageTransform;
    public Transform playerTransform;
    public Vector3 mageOffsetFromPlayer = new Vector3(1.5f, 0, 0);

    [Header("Tiempos")]
    public float fadeDuration = 0.5f;
    public float moveSpeed = 2f;

    [Header("Dialogo introductorio")]
    [TextArea(2, 6)]
    public string[] introDialogueLines;

    private Vector3 mageOriginalPos;
    private Image fadeOverlay;

    void Start()
    {
        if (GameState.Instance == null || !GameState.Instance.playIntro)
        {
            enabled = false;
            return;
        }
        GameState.Instance.playIntro = false;
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        Player_Idle.inputBlocked = true;
        mageOriginalPos = mageTransform.position;

        Animator mageAnimator = mageTransform.GetComponent<Animator>();
        NPC mageNpc = mageTransform.GetComponent<NPC>();
        if (mageNpc != null) mageNpc.enabled = false;
        if (mageAnimator != null) mageAnimator.enabled = false;

        CreateFadeOverlay();
        yield return Fade(1, 0);
        Destroy(fadeOverlay.transform.parent.gameObject);

        yield return new WaitForSeconds(0.5f);

        Vector3 target = playerTransform.position + mageOffsetFromPlayer;
        yield return MoveMage(target, moveSpeed);

        if (mageAnimator != null) mageAnimator.enabled = true;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue("Mage", introDialogueLines);
            yield return new WaitUntil(() => !DialogueManager.Instance.dialoguePanel.activeSelf);
        }

        if (mageAnimator != null) mageAnimator.enabled = false;

        yield return new WaitForSeconds(0.3f);
        yield return MoveMage(mageOriginalPos, moveSpeed * 1.5f);

        if (mageAnimator != null) mageAnimator.enabled = true;
        if (mageNpc != null) mageNpc.enabled = true;

        Player_Idle.inputBlocked = false;
    }

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

    IEnumerator MoveMage(Vector3 target, float speed)
    {
        while (Vector3.Distance(mageTransform.position, target) > 0.05f)
        {
            mageTransform.position = Vector3.MoveTowards(
                mageTransform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

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
