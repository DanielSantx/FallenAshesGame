using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Panel de Pausa")]
    public GameObject pausePanel;

    [Header("Opciones")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        pausePanel.SetActive(false);
        SetupResolutions();
        LoadSettings();
    }

    void Update()
    {
        // ESC no funciona durante dialogos
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (DialogueManager.Instance.dialoguePanel.activeSelf) return;

            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SaveSettings();
        ApplySettings();
        pausePanel.SetActive(false);
    }

    public void OnVolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    void SetupResolutions()
    {
        if (resolutionDropdown == null) return;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentIndex = i;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
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