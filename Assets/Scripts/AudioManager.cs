using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    [Header("Volumen (se guarda en PlayerPrefs)")]
    public float musicVolume = 0.5f;
    public float sfxVolume = 0.5f;

    [Header("Música")]
    public AudioClip menuMusic;
    public AudioClip castleMusic;
    public AudioClip dungeonMusic;

    [Header("Efectos de sonido")]
    public AudioClip menuClick;
    public AudioClip footsteps;
    public AudioClip textTyping;
    public AudioClip shoot;
    public AudioClip damageTaken;
    public AudioClip enemyHurt;
    public AudioClip chestOpen;
    public AudioClip doorOpen;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        LoadVolumes();
        ApplyVolumes();
    }

    void OnEnable()  { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Menu":         PlayMusic(menuMusic);    break;
            case "LoreScene":    break;
            case "Castle_MainScene": PlayMusic(castleMusic); break;
            case "DungeonScene": PlayMusic(dungeonMusic); break;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = v;
        PlayerPrefs.SetFloat("MusicVolume", v);
        ApplyVolumes();
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = v;
        PlayerPrefs.SetFloat("SFXVolume", v);
        ApplyVolumes();
    }

    void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume   = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }

    void ApplyVolumes()
    {
        if (musicSource) musicSource.volume = musicVolume;
        if (sfxSource)   sfxSource.volume   = sfxVolume;
    }
}
