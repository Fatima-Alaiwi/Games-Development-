using System.Collections;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 10;
    public bool isPlayer = false;
    public HealthBar healthBar;

    public AudioClip hurtSound;

    private AudioSource audioSource;
    private Animator animator;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = transform.root.GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (isPlayer && healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        Debug.Log(gameObject.name + " took " + amount + " damage. Health: " + currentHealth);

        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hurtSound != null && audioSource != null)
            audioSource.PlayOneShot(hurtSound);

        if (isPlayer && healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (isPlayer && CameraShake.Instance != null)
            CameraShake.Instance.Shake();

        if (currentHealth <= 0)
            Death();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (isPlayer && healthBar != null)
            healthBar.SetHealth(currentHealth);

        Debug.Log(gameObject.name + " healed " + amount + ". Health now: " + currentHealth);
    }

    public void RestoreHealth(int health, int max)
    {
        maxHealth = max;
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        isDead = false;

        if (isPlayer && healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    public void Kill() => Death();

    void Death()
    {
        if (isDead) return;
        isDead = true;

        if (isPlayer)
        {
            Debug.Log("Player is dead!");

            PlayerControllerGun gunController = GetComponentInParent<PlayerControllerGun>();
            if (gunController != null) gunController.canMove = false;

            PlayerController meleeController = GetComponentInParent<PlayerController>();
            if (meleeController != null) meleeController.canMove = false;

            StartCoroutine(DelayedLoseScreen());
        }
        else
        {
            if (animator != null)
                animator.SetTrigger("Die");

            // Disable the EnemyMoveGun so it stops attacking immediately
            EnemyMoveGun enemy = GetComponent<EnemyMoveGun>();
            if (enemy != null)
                enemy.enabled = false;

            EnemySpawnerReporter reporter = GetComponent<EnemySpawnerReporter>();
            if (reporter != null)
                reporter.ReportDeath();

            Destroy(gameObject, 3f);
        }
    }

    IEnumerator DelayedLoseScreen()
    {
        // Wait for the health bar to reach empty before showing the lose screen.
        yield return new WaitForSeconds(2f);
        LoseScreenManager.Show();
    }
}
