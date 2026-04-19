//changed the script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    int currentHealth;
    public int maxHealth;
    public bool isPlayer = false;
    public HealthBar healthBar;

    void Awake()
    {
        currentHealth = maxHealth;
        if (isPlayer && healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (isPlayer && healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Death();
    }

    void Death()
    {
        if (isPlayer)
            Debug.Log("Player is dead!");
        else
            Destroy(gameObject);
    }
}