using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public int damageAmount = 1;
    public float damageCooldown = 1f;
    private float lastDamageTime = -999f;
    
    // To check if the enemy is dead before dealing damage
    private Actor myActor;

    void Start()
    {
        myActor = GetComponentInParent<Actor>();
    }

    void OnTriggerStay(Collider other)
    {
        // 1. If it's not the player, ignore it.
        if (!other.CompareTag("Player")) return;

        // 2. If this enemy is already dead, stop dealing damage.
        if (myActor != null && myActor.currentHealth <= 0) return;

        // 3. If the cooldown hasn't passed, wait.
        if (Time.time - lastDamageTime < damageCooldown) return;

        // 4. Find the player's Actor script and deal damage.
        Actor playerActor = other.GetComponent<Actor>();
        if (playerActor == null)
        {
            playerActor = other.GetComponentInParent<Actor>();
        }

        if (playerActor != null)
        {
            lastDamageTime = Time.time;
            playerActor.TakeDamage(damageAmount);
        }
    }
}