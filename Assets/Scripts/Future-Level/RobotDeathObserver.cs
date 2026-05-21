using UnityEngine;
using UnityEngine.AI;

public class RobotDeathObserver : MonoBehaviour
{
    private RobotAI _robotAI;
    private EnemyShooting _enemyShooting;
    private NavMeshAgent _navMeshAgent;
    private Animator _correctModelAnimator;
    private bool _hasTriggeredDeath = false;

    void Awake()
    {
        _robotAI = GetComponent<RobotAI>();
        _enemyShooting = GetComponent<EnemyShooting>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        // Directly grab the animator that is driving the 3D model visuals
        _correctModelAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (_hasTriggeredDeath) return;

        Actor actor = GetComponent<Actor>();
        if (actor != null && actor.currentHealth <= 0)
        {
            TriggerRobotDeath();
        }
    }

    private void TriggerRobotDeath()
{
    _hasTriggeredDeath = true;
    Debug.Log($"<color=lime>Observer executing Death Animation directly on {gameObject.name}</color>");

    // 1. Force the correct 3D model animator to switch states immediately
    if (_correctModelAnimator != null)
    {
        _correctModelAnimator.SetTrigger("Die");
    }

    // 2. Shut down the AI loops immediately
    if (_robotAI != null) _robotAI.enabled = false;
    if (_enemyShooting != null) _enemyShooting.enabled = false;

    // 3. Halt navigation positioning
    if (_navMeshAgent != null && _navMeshAgent.enabled)
    {
        if (_navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
        }
        _navMeshAgent.enabled = false;
    }

    Collider robotCollider = GetComponent<Collider>();
    if (robotCollider != null)
    {
        robotCollider.enabled = false;
    }

    CharacterController robotController = GetComponent<CharacterController>();
    if (robotController != null)
    {
        robotController.enabled = false;
    }
}
}