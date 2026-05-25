using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================
// ReturnPortal: Portal que aparece en la sala del jefe al
// derrotarlo. Lleva al jugador de vuelta al castillo.
// ============================================================
public class ReturnPortal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SceneManager.LoadScene("Castle_MainScene");
    }
}
