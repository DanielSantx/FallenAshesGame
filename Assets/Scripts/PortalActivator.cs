using UnityEngine;

// ============================================================
// PortalActivator: Se coloca en la sala del jefe. Escucha el
// evento de muerte del jefe (Enemy.OnDeath) y, cuando ocurre,
// activa el portal de retorno al castillo.
// ============================================================
public class PortalActivator : MonoBehaviour
{
    public Enemy boss;       // Referencia al jefe (Enemy)
    public GameObject portal; // Portal de retorno (inicialmente desactivado)

    void Start()
    {
        // Se suscribe al evento de muerte del jefe
        if (boss != null)
            boss.OnDeath += ActivatePortal;
        // El portal comienza oculto
        if (portal != null)
            portal.SetActive(false);
    }

    // Activa el portal de retorno (llamado por el evento OnDeath del jefe)
    void ActivatePortal()
    {
        if (portal != null)
            portal.SetActive(true);
    }
}
