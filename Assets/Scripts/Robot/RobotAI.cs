using UnityEngine;
using UnityEngine.AI;

public class RobotAI : MonoBehaviour 
{
    [Header("Combat Settings")]
    public Transform player; 
    public float detectionRange = 25f; 
    public float stopDistance = 5f; 

    [Header("References")]
    public GameObject pistolModel; 

    private NavMeshAgent _agent;
    private EnemyShooting _shootingScript;
    private Animator _anim;

    void Start() 
    {
        _agent = GetComponent<NavMeshAgent>();
        _shootingScript = GetComponent<EnemyShooting>();
        _anim = GetComponent<Animator>();

        // Debugging Component Check
        if (_agent == null) Debug.LogError("<color=red>ROBOT ERROR:</color> NavMeshAgent is MISSING from this object!");
        if (_anim == null) Debug.LogWarning("<color=yellow>ROBOT WARNING:</color> Animator is missing!");

        if (pistolModel != null) pistolModel.SetActive(true);
        if (_anim != null) _anim.SetInteger("WeaponType", 1); 

        _agent.Warp(transform.position);
        // Auto-find player by tag
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) {
                player = p.transform;
                Debug.Log("<color=green>ROBOT OK:</color> Player found and assigned automatically.");
            } else {
                Debug.LogError("<color=red>ROBOT ERROR:</color> No object with tag 'Player' found in scene!");
            }
        }
    }

    void Update() 
    {
        if (PauseMenuManager.isPaused) 
        {
            if (_agent != null && _agent.isOnNavMesh) _agent.isStopped = true;
            if (_anim != null) _anim.speed = 0;
            return;
        }

        if (_anim != null) _anim.speed = 1;
        if (_agent == null || player == null) return;

        // Drive walking animation via physical velocity
        if (_anim != null)
        {
            float currentSpeed = _agent.velocity.magnitude;
            _anim.SetFloat("Speed", currentSpeed);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange) 
        {
            HandleChase(distanceToPlayer);
        } 
        else 
        {
            if (!_agent.isStopped) {
                Debug.Log("ROBOT: Player out of range. Stopping.");
                _agent.isStopped = true;
            }
            if (_shootingScript != null) _shootingScript.enabled = false;
        }
    }

    private void HandleChase(float distance) 
    {
        // Debug path status if not moving
        if (!_agent.hasPath && _agent.pathStatus == NavMeshPathStatus.PathInvalid) {
            Debug.LogWarning("ROBOT: I have no valid path to the player! Check your NavMesh Bake.");
        }

        if (distance > stopDistance) 
        {
            _agent.isStopped = false;
            _agent.SetDestination(player.position);
            if (_shootingScript != null) _shootingScript.enabled = false;
            
            // Helpful log to see if the agent is actually trying to move
            if (_agent.velocity.magnitude < 0.1f) {
                Debug.Log("ROBOT: Trying to move, but velocity is nearly zero. Stuck?");
            }
        } 
        else 
        {
            _agent.isStopped = true; 
            if (_shootingScript != null) _shootingScript.enabled = true;

            // Smooth rotation
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }
    }
}