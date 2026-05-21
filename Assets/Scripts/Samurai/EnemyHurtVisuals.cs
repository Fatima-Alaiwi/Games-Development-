using UnityEngine;

public class EnemyHurtVisuals : MonoBehaviour
{
    private Actor actor;
    private Animator anim;
    private int lastKnownHealth;

    void Start()
    {
        actor = GetComponent<Actor>();
        anim = GetComponentInChildren<Animator>();
        
        if (actor != null)
        {
            lastKnownHealth = actor.currentHealth;
        }
    }

    void Update()
    {
        if (actor == null || anim == null) return;

        // If current health is lower than our last recorded health, we took damage!
        if (actor.currentHealth < lastKnownHealth)
        {
            // Only play the hurt animation if we aren't dead
            if (actor.currentHealth > 0)
            {
                // Copying your teammate's exact animation logic
                anim.ResetTrigger("hurt");
                anim.CrossFadeInFixedTime("Hurt", 0.15f, 0, 0f);
            }
            
            // Update our record so we don't play it every single frame
            lastKnownHealth = actor.currentHealth;
        }
        // Handle healing just in case
        else if (actor.currentHealth > lastKnownHealth)
        {
            lastKnownHealth = actor.currentHealth;
        }
    }
}