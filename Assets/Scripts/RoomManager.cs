using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ============================================================
// RoomManager: Controla las oleadas de enemigos en una sala de
// la mazmorra. Mantiene las puertas cerradas mientras haya
// enemigos vivos y las abre al despejar todas las oleadas.
// ============================================================
public class RoomManager : MonoBehaviour
{
    // Puertas que se bloquean/desbloquean
    [Header("Puertas")]
    public GameObject[] doors;
    public AudioClip doorOpenSound; // Sonido al abrir las puertas

    // Array de oleadas (cada una con sus spawn points)
    [Header("Oleadas")]
    public Wave[] waves;
    private int currentWave = 0;    // Índice de la oleada actual
    private int aliveEnemies = 0;   // Enemigos vivos en la oleada actual

    [Header("Estado")]
    public bool roomCleared = false; // True cuando todas las oleadas están despejadas

    private bool isSpawning = false;

    void Start()
    {
        LockDoors();
        StartCoroutine(StartFirstWave());
    }

    IEnumerator StartFirstWave()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnWave(0);
    }

    void SpawnWave(int waveIndex)
    {
        if (waveIndex >= waves.Length)
        {
            RoomCleared();
            return;
        }

        Wave wave = waves[waveIndex];
        int spawned = 0;

        foreach (SpawnPoint sp in wave.spawnPoints)
        {
            if (sp.prefab == null) continue;
            GameObject enemy = Instantiate(sp.prefab, sp.position.position, sp.position.rotation);
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
                enemyScript.OnDeath += OnEnemyDied;
            spawned++;
        }

        aliveEnemies = spawned;
        isSpawning = false;
    }

    void OnEnemyDied()
    {
        if (roomCleared || isSpawning) return;
        aliveEnemies--;
        if (aliveEnemies <= 0)
        {
            currentWave++;
            if (currentWave < waves.Length)
                StartCoroutine(DelayedNextWave());
            else
                RoomCleared();
        }
    }

    IEnumerator DelayedNextWave()
    {
        isSpawning = true;
        yield return new WaitForSeconds(1f);
        SpawnWave(currentWave);
    }

    // Marca la sala como despejada y abre las puertas
    void RoomCleared()
    {
        roomCleared = true;
        UnlockDoors();
    }

    // Activa las puertas (bloquean el paso)
    void LockDoors()
    {
        foreach (GameObject door in doors)
            if (door != null) door.SetActive(true);
    }

    // Desactiva las puertas (permiten pasar) y reproduce sonido
    void UnlockDoors()
    {
        if (AudioManager.Instance != null && doorOpenSound != null)
            AudioManager.Instance.PlaySFX(doorOpenSound);
        foreach (GameObject door in doors)
            if (door != null) door.SetActive(false);
    }

    // Clase auxiliar para definir una oleada en el Inspector
    [System.Serializable]
    public class Wave
    {
        public SpawnPoint[] spawnPoints;
    }

    // Posición y prefab de cada enemigo a spawnear
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform position;
        public GameObject prefab;
    }
}
