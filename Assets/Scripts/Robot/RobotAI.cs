using UnityEngine;
using UnityEngine.AI;

public class RobotAI : MonoBehaviour 
{
    [Header("Combat Settings")]
    public Transform player; 
    public float shootingRange = 8f;
    public float navMeshSampleDistance = 10f;
    public bool useDirectMovementWhenNavMeshFails = false;
    public float directMoveSpeed = 3.5f;
    public bool debugNavMesh = true;
    public float debugLogInterval = 1f;

    [Header("References")]
    public GameObject pistolModel; 

    private NavMeshAgent _agent;
    private EnemyShooting _shootingScript;
    private Animator _anim;
    private bool _hasValidAgent;
    private bool _usingDirectMovement;
    private bool _hasLateDirectPosition;
    private Vector3 _lateDirectPosition;
    private float _nextDebugLogTime;

    void Start() 
    {
        _agent = GetComponent<NavMeshAgent>();
        _shootingScript = GetComponent<EnemyShooting>();
        _anim = GetComponent<Animator>();

        // Debugging Component Check
        if (_agent == null)
        {
            Debug.LogError("<color=red>ROBOT ERROR:</color> NavMeshAgent is MISSING from this object!");
            enabled = false;
            return;
        }

        if (_anim == null) Debug.LogWarning("<color=yellow>ROBOT WARNING:</color> Animator is missing!");
        if (_anim != null) _anim.applyRootMotion = false;

        if (pistolModel != null) pistolModel.SetActive(true);
        if (_anim != null) _anim.SetInteger("WeaponType", 1); 

        _hasValidAgent = TryPlaceOnNavMesh();
        _agent.stoppingDistance = shootingRange;

        FindPlayerTarget();
    }

    private void FindPlayerTarget()
    {
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

        if (player == null)
        {
            PlayerControllerGun gunPlayer = FindFirstObjectByType<PlayerControllerGun>();
            if (gunPlayer != null)
            {
                player = gunPlayer.transform;
                Debug.Log("<color=green>ROBOT OK:</color> PlayerControllerGun found as fallback target.");
            }
        }

        if (player == null)
        {
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                player = playerController.transform;
                Debug.Log("<color=green>ROBOT OK:</color> PlayerController found as fallback target.");
            }
        }
    }

    void Update() 
    {
        if (PauseMenuManager.isPaused) 
        {
            if (_hasValidAgent && _agent.isOnNavMesh) _agent.isStopped = true;
            if (_anim != null) _anim.speed = 0;
            return;
        }

        if (_anim != null) _anim.speed = 1;
        if (player == null)
        {
            FindPlayerTarget();
            if (player == null) return;
        }

        // Drive walking animation via physical velocity
        if (_anim != null)
        {
            float currentSpeed = _agent.velocity.magnitude;
            _anim.SetFloat("Speed", currentSpeed);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        HandleChase(distanceToPlayer);
    }

    private void LateUpdate()
    {
        if (_usingDirectMovement && _hasLateDirectPosition)
        {
            transform.position = _lateDirectPosition;
        }
    }

    private void HandleChase(float distance) 
    {
        if (_usingDirectMovement || !_hasValidAgent || !_agent.isOnNavMesh)
        {
            if (!_usingDirectMovement && TryPlaceOnNavMesh())
            {
                _hasValidAgent = true;
                _agent.stoppingDistance = shootingRange;
            }
            else
            {
                if (useDirectMovementWhenNavMeshFails)
                {
                    BeginDirectMovement();
                    HandleDirectChase(distance);
                }
                else
                {
                    StopMoving();
                }

                return;
            }
        }

        // Debug path status if not moving
        if (!_agent.hasPath && _agent.pathStatus == NavMeshPathStatus.PathInvalid) {
            Debug.LogWarning("ROBOT: I have no valid path to the player! Check your NavMesh Bake.");
        }

        if (distance > shootingRange) 
        {
            Vector3 chasePosition = player.position;
            bool foundPlayerNavMeshPoint = NavMesh.SamplePosition(player.position, out NavMeshHit targetHit, navMeshSampleDistance, NavMesh.AllAreas);
            if (foundPlayerNavMeshPoint)
            {
                chasePosition = targetHit.position;
            }

            NavMeshPath path = new NavMeshPath();
            if (!_agent.CalculatePath(chasePosition, path) || path.status == NavMeshPathStatus.PathInvalid)
            {
                LogNavMeshDebug("Invalid path", distance, chasePosition, foundPlayerNavMeshPoint, path.status);

                if (useDirectMovementWhenNavMeshFails)
                {
                    BeginDirectMovement();
                    HandleDirectChase(distance);
                }
                else
                {
                    StopMoving();
                }

                return;
            }

            _agent.isStopped = false;

            if (path.status == NavMeshPathStatus.PathPartial && path.corners.Length > 1)
            {
                chasePosition = path.corners[path.corners.Length - 1];
            }

            string chaseState = path.status == NavMeshPathStatus.PathPartial ? "Chasing partial path" : "Chasing";

            if (!_agent.SetDestination(chasePosition))
            {
                LogNavMeshDebug("SetDestination failed", distance, chasePosition, foundPlayerNavMeshPoint, path.status);

                if (useDirectMovementWhenNavMeshFails)
                {
                    BeginDirectMovement();
                    HandleDirectChase(distance);
                }
                else
                {
                    StopMoving();
                }

                return;
            }

            if (_shootingScript != null) _shootingScript.enabled = false;
            LogNavMeshDebug(chaseState, distance, chasePosition, foundPlayerNavMeshPoint, path.status);
            
            // Helpful log to see if the agent is actually trying to move
            if (_agent.velocity.magnitude < 0.1f) {
            }
        } 
        else 
        {
            _agent.isStopped = true; 
            _agent.ResetPath();
            if (_shootingScript != null) _shootingScript.enabled = true;

            // Smooth rotation
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }
    }

    private bool TryPlaceOnNavMesh()
    {
        if (_agent != null && !_agent.enabled)
        {
            _agent.enabled = true;
        }

        if (_agent.isOnNavMesh)
        {
            return true;
        }

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            _agent.Warp(hit.position);
            return true;
        }

        Debug.LogWarning($"ROBOT: {gameObject.name} spawned off the NavMesh. Move the spawn point onto baked walkable ground.");
        return false;
    }

    private void HandleDirectChase(float distance)
    {
        if (!useDirectMovementWhenNavMeshFails || player == null)
        {
            _hasLateDirectPosition = false;

            if (_shootingScript != null) _shootingScript.enabled = false;
            return;
        }

        FacePlayer();

        if (distance > shootingRange)
        {
            if (_shootingScript != null) _shootingScript.enabled = false;

            Vector3 targetPosition = player.position;
            targetPosition.y = transform.position.y;
            _lateDirectPosition = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                directMoveSpeed * Time.deltaTime);
            _hasLateDirectPosition = true;
            transform.position = _lateDirectPosition;

            if (_anim != null)
            {
                _anim.SetFloat("Speed", directMoveSpeed);
            }
        }
        else
        {
            _hasLateDirectPosition = false;

            if (_shootingScript != null) _shootingScript.enabled = true;

            if (_anim != null)
            {
                _anim.SetFloat("Speed", 0f);
            }
        }
    }

    private void BeginDirectMovement()
    {
        if (_usingDirectMovement)
        {
            return;
        }

        _usingDirectMovement = true;
        _hasValidAgent = false;
        _lateDirectPosition = transform.position;
        _hasLateDirectPosition = true;

        if (_agent != null && _agent.enabled)
        {
            if (_agent.isOnNavMesh)
            {
                _agent.ResetPath();
            }

            _agent.enabled = false;
        }
    }

    private void StopMoving()
    {
        _hasLateDirectPosition = false;

        if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }

        if (_shootingScript != null)
        {
            _shootingScript.enabled = false;
        }

        if (_anim != null)
        {
            _anim.SetFloat("Speed", 0f);
        }
    }

    private void LogNavMeshDebug(
        string state,
        float distance,
        Vector3 destination,
        bool foundPlayerNavMeshPoint,
        NavMeshPathStatus pathStatus = NavMeshPathStatus.PathInvalid)
    {
        if (!debugNavMesh || Time.time < _nextDebugLogTime)
        {
            return;
        }

        _nextDebugLogTime = Time.time + debugLogInterval;

        string agentInfo = _agent == null
            ? "agent=null"
            : $"agentEnabled={_agent.enabled}, onNavMesh={_agent.isOnNavMesh}, hasPath={_agent.hasPath}, pending={_agent.pathPending}, agentPath={_agent.pathStatus}, remaining={_agent.remainingDistance}, velocity={_agent.velocity.magnitude}";

        Debug.Log(
            $"ROBOT NAV DEBUG [{gameObject.name}] {state}: distance={distance}, foundPlayerNavMeshPoint={foundPlayerNavMeshPoint}, calculatedPath={pathStatus}, destination={destination}, {agentInfo}");
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }
    }
}
