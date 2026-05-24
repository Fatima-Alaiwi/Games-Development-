using UnityEngine;
using UnityEngine.AI;

public class EnemyUnstuck : MonoBehaviour
{
    [Header("Settings")]
    public float checkInterval = 1f;      // how often to check if stuck
    public float stuckThreshold = 0.2f;  // minimum distance moved to not be considered stuck
    public float unstuckJumpRadius = 2f; // how far to teleport when stuck

    private NavMeshAgent agent;
    private Vector3 lastPosition;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckIfStuck();
        }
    }

    void CheckIfStuck()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        // If agent has a destination but barely moved
        if (agent != null && agent.hasPath && distanceMoved < stuckThreshold)
        {
            Unstuck();
        }

        lastPosition = transform.position;
    }

    void Unstuck()
    {
        // Try to find a valid NavMesh position nearby
        Vector3 randomDirection = Random.insideUnitSphere * unstuckJumpRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, unstuckJumpRadius, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            Debug.Log($"<color=yellow>UNSTUCK:</color> {gameObject.name} warped to {hit.position}");
        }
    }
}