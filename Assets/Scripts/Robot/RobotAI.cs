using UnityEngine;
using UnityEngine.AI;

public class RobotAI : MonoBehaviour 
{
    [Header("Detection & Combat")]
    public Transform player; 
    public float stopDistance = 5f; 
    public float moveSpeed = 3.5f;

    private NavMeshAgent _agent;
    private EnemyShooting _shootingScript;

    void Start() {
        _agent = GetComponent<NavMeshAgent>();
        _shootingScript = GetComponent<EnemyShooting>();
        
        if (_agent != null) {
            _agent.speed = moveSpeed;
        }
    }

    void Update() {
        if (PauseMenuManager.isPaused) {
            if (_agent.isActiveAndEnabled) _agent.isStopped = true;
            return;
        }

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance) {
            _agent.isStopped = false;
            _agent.SetDestination(player.position);
            
            if (_shootingScript != null) _shootingScript.enabled = false; 
        } else {
            _agent.isStopped = true; 
            if (_shootingScript != null) _shootingScript.enabled = true;

            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero) {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }
        }
    }
}