using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourAddon : MonoBehaviour
{
    [Header("Facing")]
    public float rotationSpeed = 6f;

    [Header("Movement")]
    public float distanceToStop = 1.2f;

    [Header("Stuck Detection")]
    public float stuckCheckInterval = 3f;
    public float stuckMoveThreshold = 0.3f;
    public int stuckCountBeforeWarp = 2;

    private NavMeshAgent _agent;
    private Transform _player;
    private Actor _actor;
    private EnemyMove _enemyMove; // reference so we can disable it

    private Vector3 _lastPosition;
    private float _stuckTimer;
    private int _stuckCount;
    private bool _isDead;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _actor = GetComponent<Actor>();

        // Disable EnemyMove so it can't set agent.destination = self anymore
        _enemyMove = GetComponent<EnemyMove>();
        if (_enemyMove != null)
        {
            _enemyMove.enabled = false;
            Debug.Log($"[Addon] {name}: Disabled EnemyMove — addon handling movement.");
        }

        // Disable EnemyUnstuck to avoid warp conflicts
        EnemyUnstuck eu = GetComponent<EnemyUnstuck>();
        if (eu != null)
        {
            eu.enabled = false;
            Debug.Log($"[Addon] {name}: Disabled EnemyUnstuck.");
        }

        // Find player
        if (PlayerController.instance != null)
            _player = PlayerController.instance.transform;
        else
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) _player = p.transform;
        }

        if (_player == null) Debug.LogError($"[Addon] {name}: Player not found!");

        _lastPosition = transform.position;
    }

    void Update()
    {
        if (_agent == null || _player == null) return;
        if (!_agent.isOnNavMesh) return;

        // Dead
        if (_actor != null && _actor.currentHealth <= 0)
        {
            if (!_isDead)
            {
                _isDead = true;
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;

                // Re-enable EnemyMove so death animations still work
                if (_enemyMove != null) _enemyMove.enabled = true;
            }
            return;
        }

        // Paused
        if (PauseMenuManager.isPaused)
        {
            _agent.isStopped = true;
            return;
        }

        _agent.isStopped = false;

        Vector3 playerPos = _player.position;
        float dist = Vector3.Distance(transform.position, playerPos);

        // Clean stop/move logic — no self-destination bug
        if (dist > distanceToStop)
            _agent.SetDestination(playerPos);
        else
            _agent.ResetPath(); // cleanly stop without setting self as destination

        // Rotate toward player
        Vector3 dirToPlayer = playerPos - transform.position;
        dirToPlayer.y = 0f;
        if (dirToPlayer.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dirToPlayer),
                Time.deltaTime * rotationSpeed);
        }

        // Stuck detection — only when far enough that it matters
        _stuckTimer += Time.deltaTime;
        if (_stuckTimer >= stuckCheckInterval)
        {
            _stuckTimer = 0f;
            float moved = Vector3.Distance(transform.position, _lastPosition);
            _lastPosition = transform.position;

            if (dist > distanceToStop * 3f && moved < stuckMoveThreshold)
            {
                _stuckCount++;
                Debug.Log($"<color=orange>[Addon] {name} stuck " +
                          $"({_stuckCount}/{stuckCountBeforeWarp}), " +
                          $"moved: {moved:F2}, dist: {dist:F1}</color>");

                if (_stuckCount >= stuckCountBeforeWarp)
                {
                    TryUnstuck();
                    _stuckCount = 0;
                }
            }
            else
            {
                _stuckCount = 0;
            }
        }
    }

    void TryUnstuck()
    {
        Vector3 myPos = transform.position;
        Vector3 playerPos = _player.position;
        Vector3 toPlayer = (playerPos - myPos).normalized;
        float stepSize = 3f;

        Vector3[] candidates =
        {
            myPos + toPlayer * stepSize,
            myPos + toPlayer * stepSize + Vector3.Cross(toPlayer, Vector3.up) * 1.5f,
            myPos + toPlayer * stepSize - Vector3.Cross(toPlayer, Vector3.up) * 1.5f,
            myPos + toPlayer * (stepSize * 0.5f),
        };

        foreach (Vector3 candidate in candidates)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, stepSize, NavMesh.AllAreas))
            {
                if (Vector3.Distance(hit.position, myPos) < 0.5f) continue;
                _agent.Warp(hit.position);
                _agent.SetDestination(playerPos);
                Debug.Log($"<color=green>[Addon] {name} warped, " +
                          $"new dist: {Vector3.Distance(hit.position, playerPos):F1}</color>");
                return;
            }
        }

        // Emergency — teleport next to player
        NavMeshHit emergency;
        if (NavMesh.SamplePosition(playerPos, out emergency, 5f, NavMesh.AllAreas))
        {
            _agent.Warp(emergency.position);
            _agent.SetDestination(playerPos);
            Debug.LogWarning($"<color=red>[Addon] {name} emergency teleport.</color>");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceToStop);
    }
}