using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 1;

    private float lastAttackTime;
    private Animator animator;
    private Actor playerActor;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // Find the player's Actor component
        if (PlayerController.instance != null)
            playerActor = PlayerController.instance.GetComponent<Actor>();
    }

    void Update()
    {
        if (playerActor == null || PlayerController.instance == null) return;

        float distance = Vector3.Distance(
            transform.position,
            PlayerController.instance.transform.position
        );

        // Attack when in range and cooldown has passed
        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    void Attack()
    {
        if (animator != null)
            animator.SetTrigger("MeleeAttack");

        if (playerActor != null)
            playerActor.TakeDamage(attackDamage);

        Debug.Log("Enemy attacked player!");
    }
}