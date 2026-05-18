using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnPortal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SceneManager.LoadScene("Castle_MainScene");
    }
}
