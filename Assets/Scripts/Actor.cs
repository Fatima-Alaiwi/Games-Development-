using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    void Death()
    {
        if (isDead) return;
        isDead = true;

        if (isPlayer)
        {
            Debug.Log("Player is dead!");
            Scene currentScene = SceneManager.GetActiveScene();
            //SceneManager.LoadScene(currentScene.name); //raghad commented this for the health Bar
            StartCoroutine(DelayedRestart());
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

    IEnumerator DelayedRestart()
    {
        // Raghad: wait for health bar to reach empty before restarting
        yield return new WaitForSeconds(2f);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}