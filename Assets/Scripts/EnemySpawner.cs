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

    [Header("Quest Settings")]
    public Quest killQuest;

    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;
    private bool isSpawning = false;

    public void StartSpawning()
    {
        if (isSpawning) return;

        if (enemyPrefab == null)
        {
            Debug.LogWarning("No enemy prefab assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        if (killQuest != null)
            QuestManager.Instance.AcceptQuest(killQuest);

        isSpawning = true;
        enemiesSpawned = 0;
        enemiesKilled = 0;

        StartCoroutine(SpawnEnemies());
        Debug.Log("Enemy spawner started!");
    }

    IEnumerator SpawnEnemies()
    {
        while (enemiesSpawned < totalEnemiesToSpawn)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            EnemySpawnerReporter reporter = enemy.AddComponent<EnemySpawnerReporter>();
            reporter.mySpawner = this;

            enemiesSpawned++;

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        Debug.Log("All enemies spawned!");
    }

   public void OnEnemyKilled()
{
    enemiesKilled++;
    Debug.Log($"Enemies killed: {enemiesKilled}/{totalEnemiesToSpawn}");

    if (killQuest != null)
        QuestManager.Instance.UpdateProgress(killQuest.goalItemName, 1);

    if (enemiesKilled >= totalEnemiesToSpawn)
        Debug.Log("All enemies defeated! Quest complete!");
}
}