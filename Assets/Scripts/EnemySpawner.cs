using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;    // The Enemy Prefab to spawn
    public Transform[] spawnPoints;   // An array of locations where enemies can appear

    public float spawnInterval = 2f;  // Time between spawns in seconds
    public bool startSpawningOnLoad = true;

    void Start()
    {
        if (startSpawningOnLoad)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        // Infinite loop that runs until the game ends
        while (true)
        {
            // 1. Check the Game Manager: Is the player dead?
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            {
                // Stop spawning if game is over
                yield break;
            }

            // 2. Pick a random spawn point
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // 3. Spawn the enemy
            // We use Quaternion.identity for "no rotation" (let the EnemyAI handle rotation)
            Instantiate(enemyPrefab, randomPoint.position, Quaternion.identity);

            // 4. Wait for the interval before looping again
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}