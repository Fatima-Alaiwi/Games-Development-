using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int totalEnemiesToSpawn = 20;

    [Header("Spawn Settings")]
    public float timeBetweenSpawns = 1f;

    [Header("Quest Settings (Optional)")]
    public Quest killQuest; // Leave null if not using the quest system

    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;
    private bool isSpawning = false;



    public void StartSpawning()
    {
        if (isSpawning)
        {
            Debug.LogWarning("EnemySpawner: Already spawning, ignoring duplicate call.");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: No enemy prefab assigned! Assign one in the Inspector.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawner: No spawn points assigned! Add at least one in the Inspector.");
            return;
        }

        // Only interact with QuestManager if a quest is assigned AND QuestManager exists
        if (killQuest != null)
        {
            if (QuestManager.Instance != null)
                QuestManager.Instance.AcceptQuest(killQuest);
            else
                Debug.LogWarning("EnemySpawner: killQuest is assigned but QuestManager.Instance is null. Quest will not be tracked.");
        }

        isSpawning = true;
        enemiesSpawned = 0;
        enemiesKilled = 0;

        StartCoroutine(SpawnEnemies());
        Debug.Log("EnemySpawner: Spawning started.");
    }

    IEnumerator SpawnEnemies()
    {
        while (enemiesSpawned < totalEnemiesToSpawn)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            if (spawnPoint == null)
            {
                Debug.LogWarning($"EnemySpawner: Spawn point at index {randomIndex} is null, skipping.");
                yield return new WaitForSeconds(timeBetweenSpawns);
                continue;
            }

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Use GetComponent first to avoid adding a duplicate if the prefab already has the reporter
            EnemySpawnerReporter reporter = enemy.GetComponent<EnemySpawnerReporter>();
            if (reporter == null)
                reporter = enemy.AddComponent<EnemySpawnerReporter>();

            reporter.mySpawner = this;

            enemiesSpawned++;
            Debug.Log($"EnemySpawner: Spawned enemy {enemiesSpawned}/{totalEnemiesToSpawn}");

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        Debug.Log("EnemySpawner: All enemies have been spawned.");
    }

    /// <summary>
    /// Called by EnemySpawnerReporter when a spawned enemy dies.
    /// </summary>
    public void OnEnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"EnemySpawner: Enemies killed: {enemiesKilled}/{totalEnemiesToSpawn}");

        if (killQuest != null && QuestManager.Instance != null)
            QuestManager.Instance.UpdateProgress(killQuest.goalItemName, 1);

        if (enemiesKilled >= totalEnemiesToSpawn)
        {
            Debug.Log("EnemySpawner: All enemies defeated!");
            isSpawning = false;
        }
    }
}