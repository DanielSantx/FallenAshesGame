using UnityEngine;

// ============================================================
// Bullet: Proyectil disparado por el arma del jugador.
// Viaja en línea recta y se destruye al impactar con un enemigo,
// una pared o al cumplir su tiempo de vida máximo.
// El daño se asigna desde GunController.Shoot() según las
// mejoras del jugador.
// ============================================================
public class Bullet : MonoBehaviour
{
    public float lifetime = 3f; // Tiempo máximo de vida de la bala
    public int damage = 1;      // Daño que inflige (se sobrescribe desde GunController)

    void Start()
    {
        // Autodestrucción tras lifetime segundos (seguridad por si no impacta)
        Destroy(gameObject, lifetime);
    }

    // Al colisionar con un trigger (CircleCollider2D Trigger)
    void OnTriggerEnter2D(Collider2D other)
    {
        // Impacta con enemigo: le aplica daño y se destruye
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        // Impacta con pared: se destruye sin más
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
