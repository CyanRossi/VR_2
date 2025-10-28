using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWave : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] smallEnemyPrefabs;
    public GameObject[] bigEnemyPrefabs;

    [Header("Spawn Points")]
    public Transform[] spawnPoints; // Assigned via Inspector

    [Header("Wave Settings")]
    public int totalWaves = 5;
    public int smallEnemyCount = 5;
    public int bigEnemyCount = 3;
    public float timeBetweenWaves = 5f;

    private bool hasTriggered = false;
    private int currentWave = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(SpawnWaves());
        }
    }

    private IEnumerator SpawnWaves()
    {
        for (int i = 0; i < totalWaves; i++)
        {
            Debug.Log($"Spawning Wave {i + 1}");

            // Spawn small enemies
            for (int j = 0; j < smallEnemyCount; j++)
            {
                GameObject prefab = smallEnemyPrefabs[Random.Range(0, smallEnemyPrefabs.Length)];
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            }

            // Spawn big enemies
            for (int k = 0; k < bigEnemyCount; k++)
            {
                GameObject prefab = bigEnemyPrefabs[Random.Range(0, bigEnemyPrefabs.Length)];
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            }

            currentWave++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
}
