using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;
    public bool hasSpokenToKing = false;
    public bool hasSpokenToMage = false;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }
}