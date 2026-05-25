using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourAddon : MonoBehaviour
{
    [Header("Movement")]
    public float distanceToStop = 1.2f;
    public float rotationSpeed = 6f;
    public float normalSpeed = 7f;

    [Header("Stuck Recovery — no jumping")]
    public float stuckCheckInterval = 2.5f;
    public float stuckMoveThreshold = 0.25f;
    public int stuckCountBeforeBoost = 2;
    public float boostSpeed = 16f;

    // ── private ───────────────────────────────
    private NavMeshAgent _agent;
    private Transform _player;
    private Actor _actor;
    private Animator _anim;
    private EnemyMove _enemyMove;
    private EnemyAI _enemyAI;

    private Vector3 _lastPos;
    private float _stuckTimer;
    private int _stuckCount;
    private bool _isDead;
    private bool _isBoosted;

    // Track hurt state so we know when animation is safe
    private float _hurtCooldown;
    private int _lastKnownHealth;

    void Start()
    {
        _agent  = GetComponent<NavMeshAgent>();
        _actor  = GetComponent<Actor>();
        _anim   = GetComponentInChildren<Animator>(true);

        // ── Disable competing scripts ─────────
        _enemyMove = GetComponent<EnemyMove>();
        if (_enemyMove != null) _enemyMove.enabled = false;

        EnemyUnstuck eu = GetComponent<EnemyUnstuck>();
        if (eu != null) eu.enabled = false;

        // EnemyAI only handles weapon type + pause anim speed
        // We leave it enabled but override Speed ourselves
        _enemyAI = GetComponent<EnemyAI>();

        // ── Player ────────────────────────────
        if (PlayerController.instance != null)
            _player = PlayerController.instance.transform;
        else
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) _player = p.transform;
        }

        if (_agent != null)
        {
            _agent.speed        = normalSpeed;
            _agent.acceleration = 20f;
            _agent.angularSpeed = 360f;
            _agent.stoppingDistance = distanceToStop;
            _agent.autoBraking  = true;
        }

        if (_actor != null)
            _lastKnownHealth = _actor.currentHealth;

        _lastPos = transform.position;
    }

    void Update()
    {
        // Guarantee EnemyMove stays disabled every frame
        // This fixes the execution order race condition
        if (_enemyMove != null && _enemyMove.enabled)
            _enemyMove.enabled = false;

        if (_agent == null || _player == null) return;
        if (!_agent.isOnNavMesh) return;

        // ── Dead ──────────────────────────────
        if (_actor != null && _actor.currentHealth <= 0)
        {
            if (!_isDead)
            {
                _isDead = true;
                _agent.isStopped = true;
                _agent.ResetPath();
                if (_anim != null) _anim.SetFloat("Speed", 0f);
                // Re-enable EnemyMove only for death so its
                // death-related logic can still fire if needed
                if (_enemyMove != null) _enemyMove.enabled = true;
            }
            return;
        }

        // ── Paused ────────────────────────────
        if (PauseMenuManager.isPaused)
        {
            _agent.isStopped = true;
            if (_anim != null) _anim.SetFloat("Speed", 0f);
            return;
        }

        _agent.isStopped = false;

        // ── Hurt animation cooldown ───────────
        // Don't override Speed while hurt animation is playing
        // so it can complete before walk resumes
        if (_actor != null && _actor.currentHealth < _lastKnownHealth)
        {
            _lastKnownHealth = _actor.currentHealth;
            _hurtCooldown = 0.4f; // matches CrossFadeInFixedTime(0.15f) + buffer
        }

        if (_hurtCooldown > 0f)
        {
            _hurtCooldown -= Time.deltaTime;
            // Still chase during hurt — just don't override Speed
            _agent.SetDestination(_player.position);
            return;
        }

        // ── Movement ──────────────────────────
        Vector3 playerPos = _player.position;
        float dist = Vector3.Distance(transform.position, playerPos);

        if (dist > distanceToStop)
            _agent.SetDestination(playerPos);
        else
            _agent.ResetPath();

        // ── Rotation ──────────────────────────
        Vector3 dir = playerPos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * rotationSpeed);
        }

        // ── Animator Speed ────────────────────
        // Use velocity.magnitude not desiredVelocity
        // desiredVelocity is unreliable during path recalculation
        if (_anim != null)
            _anim.SetFloat("Speed",
                _agent.velocity.magnitude,
                0.1f,           // damp time — smooths out animation pops
                Time.deltaTime);

        // ── Stuck detection ───────────────────
        _stuckTimer += Time.deltaTime;
        if (_stuckTimer >= stuckCheckInterval)
        {
            _stuckTimer = 0f;
            float moved = Vector3.Distance(transform.position, _lastPos);
            _lastPos = transform.position;

            bool shouldBeMoving = dist > distanceToStop * 2f;
            bool isStuck = moved < stuckMoveThreshold;

            if (shouldBeMoving && isStuck)
            {
                _stuckCount++;
                Debug.Log($"<color=orange>[Addon] {name} stuck " +
                          $"{_stuckCount}/{stuckCountBeforeBoost} | " +
                          $"moved:{moved:F2} dist:{dist:F1}</color>");

                if (_stuckCount >= stuckCountBeforeBoost && !_isBoosted)
                {
                    _isBoosted = true;
                    _agent.speed        = boostSpeed;
                    _agent.acceleration = 50f;
                    // Force a fresh path calculation
                    _agent.ResetPath();
                    _agent.SetDestination(playerPos);
                    Debug.Log($"<color=yellow>[Addon] {name} speed boosted.</color>");
                }
            }
            else
            {
                if (_isBoosted)
                {
                    _isBoosted = false;
                    _agent.speed        = normalSpeed;
                    _agent.acceleration = 20f;
                    Debug.Log($"<color=green>[Addon] {name} speed restored.</color>");
                }
                _stuckCount = 0;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceToStop);
    }
}