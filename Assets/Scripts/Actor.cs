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
        //animator = transform.root.GetComponentInChildren<Animator>();

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
        isDead = true;

        if (isPlayer)
        {
            Debug.Log("Player is dead!");
        }
        else
        {
            if (animator != null)
                animator.SetTrigger("Die");

            //       // Tell the spawner this enemy died  Raghaddddddddddddddddddddddddd
            // EnemySpawnerReporter reporter = GetComponent<EnemySpawnerReporter>();
            // if (reporter != null)
            //     reporter.ReportDeath();
            //     //raghad

            Destroy(gameObject, 3f);
        }
    }
}