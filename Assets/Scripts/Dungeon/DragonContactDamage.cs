using UnityEngine;

public class DragonContactDamage : MonoBehaviour
{
    public int damageAmount = 1;
    public float damageCooldown = 1f; // seconds between hits

    private float lastDamageTime = -999f;
    private DragonHealth dragonHealth;

    void Start()
    {
        dragonHealth = GetComponentInParent<DragonHealth>();
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Don't damage if dragon is already dead
        if (dragonHealth != null && dragonHealth.IsDead()) return;

        if (Time.time - lastDamageTime < damageCooldown) return;
        lastDamageTime = Time.time;

        Actor playerActor = other.GetComponent<Actor>();
        if (playerActor != null)
            playerActor.TakeDamage(damageAmount);
    }
}