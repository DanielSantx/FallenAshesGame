using UnityEngine;

public class PortalActivator : MonoBehaviour
{
    public Enemy boss;
    public GameObject portal;

    void Start()
    {
        if (boss != null)
            boss.OnDeath += ActivatePortal;
        if (portal != null)
            portal.SetActive(false);
    }

    void ActivatePortal()
    {
        if (portal != null)
            portal.SetActive(true);
    }
}
