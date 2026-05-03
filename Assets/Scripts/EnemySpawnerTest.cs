using UnityEngine;

public class EnemySpawnerTest : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public float timeToSpawn;
    private float spawnCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnCounter = timeToSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0) 
        {
            spawnCounter = timeToSpawn;

            Instantiate(enemyToSpawn,transform.position,transform.rotation);
        }
    }
}
