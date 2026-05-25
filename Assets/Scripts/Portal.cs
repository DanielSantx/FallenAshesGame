using UnityEngine;

// ============================================================
// Portal: Portal del castillo que lleva a la mazmorra.
// Solo se activa si el jugador ha hablado con el Rey
// (hasSpokenToKing == true). Los guardias se encargan del
// diálogo de bloqueo si no se ha cumplido la condición.
// ============================================================
public class Portal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameState.Instance == null) return;

        if (!GameState.Instance.hasSpokenToKing)
            return;

        UnityEngine.SceneManagement.SceneManager.LoadScene("DungeonScene");
    }
}