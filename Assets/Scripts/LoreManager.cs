using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

// ============================================================
// LoreSlide: Una diapositiva del carrusel de lore, con una
// imagen a pantalla completa y un texto de narración debajo.
// ============================================================
[System.Serializable]
public class LoreSlide
{
    public Texture2D image;      // Imagen de fondo de la diapositiva
    [TextArea(3, 6)]
    public string text;          // Texto narrativo (parte inferior)
}

// ============================================================
// LoreManager: Carrusel de imágenes de lore que se muestra
// al iniciar una partida nueva. Muestra cada imagen durante
// un tiempo, con fundidos a negro entre ellas. Se puede saltar
// con Espacio / E / clic. Al terminar, activa la bandera
// flagPlayIntro para que la cinemática del mago se ejecute.
// ============================================================
public class LoreManager : MonoBehaviour
{
    // Static flag que persiste entre escenas: indica a
    // CinematicManager que debe reproducir la intro del mago
    public static bool flagPlayIntro = false;

    // Array de diapositivas (imagen + texto)
    [Header("Slides")]
    public LoreSlide[] slides;

    // UI
    [Header("UI")]
    public RawImage displayImage; // Renderiza la imagen actual
    public TMP_Text loreText;     // Texto narrativo
    public Image fadeOverlay;     // Capa negra para los fundidos

    // Tiempos de cada diapositiva y transición
    [Header("Tiempos")]
    public float slideDuration = 5f;  // Segundos que se muestra cada slide
    public float fadeDuration = 0.5f; // Duración del fundido a negro

    // Escena a cargar al terminar el carrusel
    [Header("Destino")]
    public string nextScene = "Castle_MainScene";

    private int currentSlide = 0;
    private bool skipping = false; // Evita múltiples saltos simultáneos

    void Start()
    {
        // Empieza con la pantalla en negro
        fadeOverlay.color = Color.black;
        StartCoroutine(PlayLore());
    }

    void Update()
    {
        // Salta el carrusel con Espacio, E o clic del ratón
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            if (!skipping)
            {
                skipping = true;
                StopAllCoroutines();  // Detiene PlayLore si está en curso
                StartCoroutine(SkipToEnd());
            }
        }
    }

    // Reproduce todas las diapositivas en secuencia con fundidos
    IEnumerator PlayLore()
    {
        for (currentSlide = 0; currentSlide < slides.Length; currentSlide++)
        {
            // Muestra la imagen y texto de la diapositiva actual
            displayImage.texture = slides[currentSlide].image;
            loreText.text = slides[currentSlide].text;

            // Fundido de entrada (negro → imagen)
            yield return StartCoroutine(Fade(1f, 0f));

            // Espera el tiempo configurado
            yield return new WaitForSecondsRealtime(slideDuration);

            // Fundido de salida (imagen → negro)
            yield return StartCoroutine(Fade(0f, 1f));
        }

        // Todas las diapositivas mostradas → pasar al juego
        EndLore();
    }

    // Animación de fundido: from → to en el canal alpha
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

    // Skip: funde a negro y termina
    IEnumerator SkipToEnd()
    {
        yield return StartCoroutine(Fade(0f, 1f));
        EndLore();
    }

    // Activa la bandera de intro y carga la escena del castillo
    void EndLore()
    {
        flagPlayIntro = true;
        SceneManager.LoadScene(nextScene);
    }
}
