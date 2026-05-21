using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [Header("Puertas")]
    public GameObject[] doors;
    public AudioClip doorOpenSound;

    [Header("Oleadas")]
    public Wave[] waves;
    private int currentWave = 0;
    private int aliveEnemies = 0;

    [Header("Estado")]
    public bool roomCleared = false;

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
        aliveEnemies = wave.spawnPoints.Length;

        foreach (SpawnPoint sp in wave.spawnPoints)
        {
            if (sp.prefab == null) continue;
            GameObject enemy = Instantiate(sp.prefab, sp.position.position, sp.position.rotation);

            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
                enemyScript.OnDeath += OnEnemyDied;
        }
    }

    void OnEnemyDied()
    {
        aliveEnemies--;
        if (aliveEnemies <= 0)
        {
            currentWave++;
            if (currentWave < waves.Length)
                SpawnWave(currentWave);
            else
                RoomCleared();
        }
    }

    void RoomCleared()
    {
        roomCleared = true;
        UnlockDoors();
    }

    void LockDoors()
    {
        foreach (GameObject door in doors)
            if (door != null) door.SetActive(true);
    }

    void UnlockDoors()
    {
        if (AudioManager.Instance != null && doorOpenSound != null)
            AudioManager.Instance.PlaySFX(doorOpenSound);
        foreach (GameObject door in doors)
            if (door != null) door.SetActive(false);
    }

    [System.Serializable]
    public class Wave
    {
        public SpawnPoint[] spawnPoints;
    }

    [System.Serializable]
    public class SpawnPoint
    {
        public Transform position;
        public GameObject prefab;
    }
}
