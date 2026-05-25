using UnityEngine;

// ============================================================
// CameraFollow2D: Cámara que sigue al jugador en 2D.
// Se actualiza en LateUpdate para evitar el tearing visual.
// Mantiene la coordenada Z original de la cámara.
// ============================================================
public class CameraFollow2D : MonoBehaviour
{
    public Player_Idle jugador; // Referencia al jugador a seguir

    // LateUpdate se ejecuta después de Update, asegurando que
    // el jugador ya se haya movido antes de que la cámara lo siga
    void LateUpdate()
    {
        transform.position = new Vector3(
            jugador.transform.position.x,
            jugador.transform.position.y,
            transform.position.z // Mantiene la Z original
        );
    }
}