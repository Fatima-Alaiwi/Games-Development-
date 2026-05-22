using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveGun : MonoBehaviour
{
    public float moveSpeed, distanceToStop;
    public Rigidbody theRigidbody;
    public NavMeshAgent agent;

    [Header("Attack")]
    public float attackRange = 1.5f;
    public int damageAmount = 1;
    public float damageCooldown = 1f;
    private float lastDamageTime = -999f;

    private Vector3 target;

    void Update()
    {
        if (PlayerControllerGun.instance == null) return;

        target = PlayerControllerGun.instance.transform.position;

        if (Vector3.Distance(transform.position, target) > distanceToStop)
            agent.destination = target;
        else
            agent.destination = transform.position;

        // Check distance to player every frame
        if (Vector3.Distance(transform.position, target) <= attackRange)
        {
            TryDamagePlayer();
        }
    }

    void TryDamagePlayer()
    {
        if (Time.time - lastDamageTime < damageCooldown) return;

        Actor playerActor = PlayerControllerGun.instance.GetComponent<Actor>();
        if (playerActor == null)
            playerActor = PlayerControllerGun.instance.GetComponentInParent<Actor>();

        if (playerActor != null)
        {
            playerActor.TakeDamage(damageAmount);
            lastDamageTime = Time.time;
            Debug.Log("Player hurt!");
        }
    }
}