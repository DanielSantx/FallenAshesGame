using UnityEngine;

// ============================================================
// EnemyHealth: Script alternativo / legacy para gestión de vida
// de enemigos (sin animaciones ni eventos). Actualmente no se
// usa en el juego: la vida de los enemigos se gestiona desde
// Enemy.cs. Se mantiene por compatibilidad.
// ============================================================
public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Aplica daño y destruye el objeto si la vida llega a 0
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Destroy(gameObject);
    }
}
