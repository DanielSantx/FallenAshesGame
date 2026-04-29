using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Player_Idle jugador;

    void LateUpdate()
    {
        transform.position = new Vector3(jugador.transform.position.x, jugador.transform.position.y, transform.position.z);
    }
}