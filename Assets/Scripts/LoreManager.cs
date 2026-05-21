using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

[System.Serializable]
public class LoreSlide
{
    public Texture2D image;
    [TextArea(3, 6)]
    public string text;
}

public class LoreManager : MonoBehaviour
{
    [Header("Slides")]
    public LoreSlide[] slides;

    [Header("UI")]
    public RawImage displayImage;
    public TMP_Text loreText;
    public Image fadeOverlay;

    [Header("Tiempos")]
    public float slideDuration = 5f;
    public float fadeDuration = 0.5f;

    [Header("Destino")]
    public string nextScene = "Castle_MainScene";

    private int currentSlide = 0;
    private bool skipping = false;

    void Start()
    {
        fadeOverlay.color = Color.black;
        StartCoroutine(PlayLore());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            if (!skipping)
            {
                skipping = true;
                StopAllCoroutines();
                StartCoroutine(SkipToEnd());
            }
        }
    }

    IEnumerator PlayLore()
    {
        for (currentSlide = 0; currentSlide < slides.Length; currentSlide++)
        {
            displayImage.texture = slides[currentSlide].image;
            loreText.text = slides[currentSlide].text;

            yield return StartCoroutine(Fade(1f, 0f));

            yield return new WaitForSecondsRealtime(slideDuration);

            yield return StartCoroutine(Fade(0f, 1f));
        }

        EndLore();
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

    IEnumerator SkipToEnd()
    {
        yield return StartCoroutine(Fade(0f, 1f));
        EndLore();
    }

    void EndLore()
    {
        if (GameState.Instance != null)
            GameState.Instance.playIntro = true;
        SceneManager.LoadScene(nextScene);
    }
}
