using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================
// AudioManager: Singleton persistente (DontDestroyOnLoad) que
// gestiona toda la reproducción de audio del juego: música de
// fondo por escena y efectos de sonido (SFX). Los volúmenes
// se guardan en PlayerPrefs y se pueden ajustar desde las
// opciones.
// ============================================================
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // Fuentes de audio dedicadas
    private AudioSource musicSource; // Música de fondo (loop)
    private AudioSource sfxSource;   // Efectos de sonido (one-shot)

    // Volumen actual (persistente en PlayerPrefs)
    [Header("Volumen (se guarda en PlayerPrefs)")]
    public float musicVolume = 0.5f;
    public float sfxVolume = 0.5f;

    // Clips de música para cada escena
    [Header("Música")]
    public AudioClip menuMusic;
    public AudioClip castleMusic;
    public AudioClip dungeonMusic;

    // Clips de efectos de sonido (asignados desde el Inspector)
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
        // Singleton: si ya existe, se destruye esta copia
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persiste entre escenas

        // Crea los AudioSources en tiempo de ejecución
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;       // La música se repite
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;        // Los SFX se reproducen una vez
        sfxSource.playOnAwake = false;

        // Carga los volúmenes guardados en PlayerPrefs
        LoadVolumes();
        ApplyVolumes();
    }

    // Se suscribe al evento de carga de escena para cambiar la música
    void OnEnable()  { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    // Cambia la música de fondo según la escena cargada
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Menu":         PlayMusic(menuMusic);    break;
            case "LoreScene":    break; // Silencio durante el carrusel
            case "Castle_MainScene": PlayMusic(castleMusic); break;
            case "DungeonScene": PlayMusic(dungeonMusic); break;
        }
    }

    // Reproduce un clip de música en bucle (si ya está sonando, lo ignora)
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    // Reproduce un efecto de sonido una vez (PlayOneShot permite solapamiento)
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // ==========================================
    // Control de volumen
    // ==========================================

    // Ajusta el volumen de la música y lo persiste
    public void SetMusicVolume(float v)
    {
        musicVolume = v;
        PlayerPrefs.SetFloat("MusicVolume", v);
        ApplyVolumes();
    }

    // Ajusta el volumen de los SFX y lo persiste
    public void SetSFXVolume(float v)
    {
        sfxVolume = v;
        PlayerPrefs.SetFloat("SFXVolume", v);
        ApplyVolumes();
    }

    // Carga los volúmenes desde PlayerPrefs (valores por defecto: 0.5)
    void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume   = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }

    // Aplica los volúmenes a los AudioSources
    void ApplyVolumes()
    {
        if (musicSource) musicSource.volume = musicVolume;
        if (sfxSource)   sfxSource.volume   = sfxVolume;
    }
}
