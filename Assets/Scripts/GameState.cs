using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    [Header("Story Progress")]
    public bool hasSpokenToKing = false;
    public bool hasSpokenToMage = false;

    [Header("Runtime")]
    public bool playIntro = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================
    // SAVE SYSTEM
    // =========================

    public void SaveGame()
    {
        PlayerPrefs.SetInt("HasSpokenToKing", hasSpokenToKing ? 1 : 0);
        PlayerPrefs.SetInt("HasSpokenToMage", hasSpokenToMage ? 1 : 0);

        PlayerPrefs.SetInt("HasSave", 1);

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        hasSpokenToKing =
            PlayerPrefs.GetInt("HasSpokenToKing", 0) == 1;

        hasSpokenToMage =
            PlayerPrefs.GetInt("HasSpokenToMage", 0) == 1;
    }

    public static bool HasSaveGame()
    {
        return PlayerPrefs.GetInt("HasSave", 0) == 1;
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
    }
}