using UnityEngine;

// ============================================================
// GameState: Singleton persistente que guarda el progreso de la
// historia (banderas narrativas) y las mejoras permanentes
// (almas, niveles de mejora) mediante PlayerPrefs.
// Al iniciar partida nueva se borra todo; al continuar se carga.
// Si no existe en la escena, se crea automáticamente la primera
// vez que se accede a Instance.
// ============================================================
public class GameState : MonoBehaviour
{
    // Instancia única del singleton (DontDestroyOnLoad)
    public static GameState Instance;

    // Crea la instancia singleton si aún no existe
    public static void EnsureInstance()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("GameState");
            DontDestroyOnLoad(go);
            go.AddComponent<GameState>();
        }
    }

    // Banderas que indican con qué NPCs ha hablado el jugador
    [Header("Story Progress")]
    public bool hasSpokenToKing = false;
    public bool hasSpokenToMage = false;

    // ==========================================
    // SISTEMA DE ALMAS Y MEJORAS (Roguelike)
    // ==========================================
    [Header("Souls & Upgrades")]
    public int souls = 0;             // Almas acumuladas (persisten entre runs)
    public int damageLevel = 0;       // Nivel de daño (0-5)
    public int fireRateLevel = 0;     // Nivel de cadencia (0-5)
    public int speedLevel = 0;        // Nivel de velocidad (0-5)
    public int maxHpLevel = 0;        // Nivel de vida máxima (0-5)

    public const int MAX_UPGRADE_LEVEL = 5;
    public static readonly int[] UPGRADE_COSTS = new int[] { 3, 5, 8, 12, 18 };

    private void Awake()
    {
        // Si no hay instancia, esta es la primera → se vuelve singleton
        if (Instance == null)
        {
            Instance = this;
            // Persiste al cambiar de escena (Menu → Lore → Castle → Dungeon)
            DontDestroyOnLoad(gameObject);
            // Carga los datos guardados en PlayerPrefs
            LoadGame();
        }
        else
        {
            // Si ya existe otro GameState, este es duplicado → se destruye
            Destroy(gameObject);
        }
    }

    // ==========================================
    // ALMAS
    // ==========================================

    // Añade almas (llamado por Soul.cs al recoger)
    public void AddSouls(int amount)
    {
        souls += amount;
    }

    // Gasta almas si hay suficiente, devuelve true si pudo
    public bool SpendSouls(int amount)
    {
        if (souls < amount) return false;
        souls -= amount;
        return true;
    }

    // Devuelve el coste de una mejora para el nivel dado
    public static int GetUpgradeCost(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= UPGRADE_COSTS.Length) return 999;
        return UPGRADE_COSTS[levelIndex];
    }

    // =========================
    // SAVE SYSTEM (PlayerPrefs)
    // =========================

    // Guarda toda la progresión en disco
    public void SaveGame()
    {
        // Flags narrativos
        PlayerPrefs.SetInt("HasSpokenToKing", hasSpokenToKing ? 1 : 0);
        PlayerPrefs.SetInt("HasSpokenToMage", hasSpokenToMage ? 1 : 0);
        // Almas y mejoras
        PlayerPrefs.SetInt("Souls", souls);
        PlayerPrefs.SetInt("DamageLevel", damageLevel);
        PlayerPrefs.SetInt("FireRateLevel", fireRateLevel);
        PlayerPrefs.SetInt("SpeedLevel", speedLevel);
        PlayerPrefs.SetInt("MaxHpLevel", maxHpLevel);
        // Marca de partida guardada (botón Continuar)
        PlayerPrefs.SetInt("HasSave", 1);
        PlayerPrefs.Save();
    }

    // Carga todos los datos guardados (llamado en Awake)
    public void LoadGame()
    {
        hasSpokenToKing = PlayerPrefs.GetInt("HasSpokenToKing", 0) == 1;
        hasSpokenToMage = PlayerPrefs.GetInt("HasSpokenToMage", 0) == 1;
        souls         = PlayerPrefs.GetInt("Souls", 0);
        damageLevel   = PlayerPrefs.GetInt("DamageLevel", 0);
        fireRateLevel = PlayerPrefs.GetInt("FireRateLevel", 0);
        speedLevel    = PlayerPrefs.GetInt("SpeedLevel", 0);
        maxHpLevel    = PlayerPrefs.GetInt("MaxHpLevel", 0);
    }

    // Consulta si existe una partida guardada (botón Continuar en el menú)
    public static bool HasSaveGame()
    {
        return PlayerPrefs.GetInt("HasSave", 0) == 1;
    }

    // Borra toda la partida guardada (partida nueva)
    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
        // También resetea las variables en memoria para esta sesión
        souls = 0;
        damageLevel = 0;
        fireRateLevel = 0;
        speedLevel = 0;
        maxHpLevel = 0;
        hasSpokenToKing = false;
        hasSpokenToMage = false;
    }
}