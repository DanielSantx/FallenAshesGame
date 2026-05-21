using UnityEngine;

public class Portal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!GameState.Instance.hasSpokenToKing)
        {
            // los guardias ya le habr�n dicho que no puede pasar,
            // aqu� simplemente no hacemos nada (el guardia tiene su di�logo)
            return;
        }

        // Cargar escena de mazmorra
        UnityEngine.SceneManagement.SceneManager.LoadScene("DungeonScene");
    }
}