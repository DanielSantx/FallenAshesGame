using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Botones")]
    public Button continueButton;
    public Button newGameButton;
    public Button optionsButton;
    public Button exitButton;

    [Header("Colores")]
    public Color colorActivo = Color.white;
    public Color colorDesactivado = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Panel de Opciones")]
    public GameObject optionsPanel;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Button backButton;

    private Resolution[] resolutions;

    void Start()
    {
        bool haySave = GameState.HasSaveGame();
        continueButton.interactable = haySave;
        TMP_Text continueTxt = continueButton.GetComponentInChildren<TMP_Text>();
        if (continueTxt) continueTxt.color = haySave ? colorActivo : colorDesactivado;

        if (optionsPanel) optionsPanel.SetActive(false);

        SetupResolutions();
        LoadSettings();
    }

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
    void LoadSettings()
    {
        if (musicVolumeSlider)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        if (sfxVolumeSlider)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        if (fullscreenToggle)
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
    }
    public void OnContinuar()
    {
        SceneManager.LoadScene("Castle_MainScene");
    }
    public void OnNuevaPartida()
    {
        if (GameState.Instance != null)
            GameState.Instance.DeleteSave();
        else
            PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Castle_MainScene");
    }
    public void OnOpciones()
    {
        if (optionsPanel) optionsPanel.SetActive(true);
    }
    public void OnBackFromOptions()
    {
        SaveSettings();
        ApplySettings();
        if (optionsPanel) optionsPanel.SetActive(false);
    }
    public void OnSalir()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
    public void SetResolution(int index)
    {
        if (resolutions != null && index < resolutions.Length)
        {
            Resolution res = resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }
    }
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
    void ApplySettings()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
    }
}