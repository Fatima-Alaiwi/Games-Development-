using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    int currentHealth;
    public int maxHealth;
    public bool isPlayer = false;
    public HealthBar healthBar;

    private Animator animator;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();

        if (isPlayer && healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (isPlayer && healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Death();
    }

    void Death()
    {
        if (isDead) return; // Extra guard — prevents Death() from firing twice
        isDead = true;

        if (isPlayer)
        {
            Debug.Log("Actor: Player is dead!");
        }
        else
        {
            // Play death animation if available
            if (animator != null)
                animator.SetTrigger("Die");

            // Report death to the spawner system (only if this enemy was spawned by an EnemySpawner)
            EnemySpawnerReporter reporter = GetComponent<EnemySpawnerReporter>();
            if (reporter != null)
                reporter.ReportDeath();

            Destroy(gameObject, 3f);
        }
    }
}