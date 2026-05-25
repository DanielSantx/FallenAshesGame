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

        // Si no ha hablado con el rey, no pasa nada
        // (los guardias ya le negarán el paso con su diálogo)
        if (!GameState.Instance.hasSpokenToKing)
        {
            return;
        }

        // Carga la escena de la mazmorra
        UnityEngine.SceneManagement.SceneManager.LoadScene("DungeonScene");
    }
}