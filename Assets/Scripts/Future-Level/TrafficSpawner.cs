using UnityEngine;
using System.Collections.Generic;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<GameObject> carPrefabs; 
    public float spawnInterval = 2f;    
    public float baseSpeed = 20f;        
    public float speedVariation = 5f;   
    public float despawnDistance = 100f; 

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnCar();
            timer = 0f;
        }
    }

    void SpawnCar()
    {
        if (carPrefabs.Count == 0) return;

        int randomIndex = Random.Range(0, carPrefabs.Count);
        GameObject carToSpawn = carPrefabs[randomIndex];

        GameObject spawnedCar = Instantiate(carToSpawn, transform.position, transform.rotation);

        // Calculate a unique speed for this specific car
        float randomSpeed = baseSpeed + Random.Range(-speedVariation, speedVariation);

        FlyingCarMovement movement = spawnedCar.AddComponent<FlyingCarMovement>();
        movement.Setup(randomSpeed, despawnDistance);
    }
}

public class FlyingCarMovement : MonoBehaviour
{
    private float speed;
    private float maxDistance;
    private Vector3 startPos;
    private LightTrailAnchor[] trailAnchors;

    public void Setup(float carSpeed, float distance)
    {
        speed = carSpeed;
        maxDistance = distance;
        startPos = transform.position;

        trailAnchors = GetComponentsInChildren<LightTrailAnchor>();
        
        foreach (var anchor in trailAnchors)
        {
            anchor.SetEmitting(true);
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}