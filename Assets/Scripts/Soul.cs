using UnityEngine;

// ============================================================
// Soul: Alma que dropean los enemigos al morir. Flota con un
// movimiento sinusoidal y se recoge al tocar al jugador.
// Se suma al contador de almas en GameState y se destruye.
// ============================================================
public class Soul : MonoBehaviour
{
    public int value = 1;                     // Almas que otorga al recogerlo
    public float floatSpeed = 2f;             // Velocidad del flote
    public float floatHeight = 0.3f;          // Amplitud del flote
    public float lifetime = 6f;               // Segundos antes de desaparecer solo

    private Vector3 basePosition;

    void Start()
    {
        basePosition = transform.position;
        // Autodestrucción si nadie lo recoge a tiempo
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Flota arriba y abajo suavemente
        Vector3 pos = basePosition;
        pos.y += Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Suma las almas al GameState, guarda y se destruye
        if (GameState.Instance != null)
        {
            GameState.Instance.AddSouls(value);
            GameState.Instance.SaveGame();
        }
        Destroy(gameObject);
    }
}
