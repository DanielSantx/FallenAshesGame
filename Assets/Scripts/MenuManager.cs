using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

// ============================================================
// MenuManager: Gestiona la escena del menú principal (botones,
// panel de opciones, carga de partida, sonidos de interfaz).
// ============================================================
public class MenuManager : MonoBehaviour
{
    // Referencias a los botones del menú principal
    [Header("Botones")]
    public Button continueButton;
    public Button newGameButton;
    public Button optionsButton;
    public Button exitButton;

    // Colores para el texto del botón Continuar (activo / desactivado)
    [Header("Colores")]
    public Color colorActivo = Color.white;
    public Color colorDesactivado = new Color(0.4f, 0.4f, 0.4f, 1f);

    // Sonidos de interfaz (click, abrir opciones)
    [Header("Sonidos")]
    public AudioClip menuClickSound;
    public AudioClip optionsOpenSound;

    // Panel de opciones con sus controles
    [Header("Panel de Opciones")]
    public GameObject optionsPanel;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Button backButton;

    // Lista de resoluciones disponibles (se obtiene de Screen.resolutions)
    private Resolution[] resolutions;

    void Start()
    {
        // El botón Continuar solo está activo si hay partida guardada
        bool haySave = GameState.HasSaveGame();
        continueButton.interactable = haySave;
        TMP_Text continueTxt = continueButton.GetComponentInChildren<TMP_Text>();
        if (continueTxt) continueTxt.color = haySave ? colorActivo : colorDesactivado;

        // Oculta el panel de opciones al inicio
        if (optionsPanel) optionsPanel.SetActive(false);

        SetupResolutions();
        LoadSettings();

        // Conecta los controles del panel de opciones a sus métodos
        // Los sliders de volumen tienen rango forzado 0-1
        if (musicVolumeSlider) { musicVolumeSlider.minValue = 0f; musicVolumeSlider.maxValue = 1f; musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume); }
        if (sfxVolumeSlider)   { sfxVolumeSlider.minValue = 0f;   sfxVolumeSlider.maxValue = 1f;   sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume); }
        if (resolutionDropdown) resolutionDropdown.onValueChanged.AddListener(SetResolution);
        if (fullscreenToggle)   fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    // Obtiene las resoluciones disponibles y llena el dropdown
    void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        if (resolutionDropdown == null) return;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
            // Marca la resolución actual como seleccionada por defecto
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // Carga valores guardados en PlayerPrefs sin disparar eventos
    void LoadSettings()
    {
        if (musicVolumeSlider)
            musicVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume", 0.5f));
        if (sfxVolumeSlider)
            sfxVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFXVolume", 0.5f));
        if (fullscreenToggle)
            fullscreenToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("Fullscreen", 1) == 1);
    }

    // Reproduce el sonido de click genérico del menú
    void PlayClick()
    {
        if (AudioManager.Instance != null && menuClickSound != null)
            AudioManager.Instance.PlaySFX(menuClickSound);
    }

    // Botón Continuar: carga la escena del castillo con la partida guardada
    public void OnContinuar()
    {
        PlayClick();
        SceneManager.LoadScene("Castle_MainScene");
    }

    // Botón Nueva Partida: borra la partida guardada y va al carrusel de Lore
    public void OnNuevaPartida()
    {
        PlayClick();
        if (GameState.Instance != null)
            GameState.Instance.DeleteSave();
        else
            PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("LoreScene");
    }

    // Botón Opciones: muestra el panel de opciones
    public void OnOpciones()
    {
        PlayClick();
        if (optionsOpenSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(optionsOpenSound);
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    // Botón Atrás (desde opciones): guarda y aplica la configuración
    public void OnBackFromOptions()
    {
        PlayClick();
        SaveSettings();
        ApplySettings();
        if (optionsPanel) optionsPanel.SetActive(false);
    }

    // Botón Salir: cierra el juego (o detiene el editor)
    public void OnSalir()
    {
        PlayClick();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // ==========================================
    // Métodos para los eventos de los controles
    // ==========================================

    // Cambia el volumen de la música y lo guarda
    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    // Cambia el volumen de los SFX y lo guarda
    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    // Cambia el modo pantalla completa / ventana
    public void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    // Cambia la resolución de la pantalla
    public void SetResolution(int index)
    {
        if (resolutions != null && index < resolutions.Length)
        {
            Resolution res = resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }
    }

    // Guarda todos los valores del panel de opciones en PlayerPrefs
    void SaveSettings()
    {
        if (musicVolumeSlider)
            PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        if (sfxVolumeSlider)
            PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        if (fullscreenToggle)
            PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Aplica la configuración de pantalla completa
    void ApplySettings()
    {
        Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
    }
}