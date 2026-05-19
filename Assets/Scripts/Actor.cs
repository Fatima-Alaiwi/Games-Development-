using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Actor : MonoBehaviour
{
    int currentHealth;
    public int maxHealth = 10;
    public bool isPlayer = false;
    public HealthBar healthBar;

    public AudioClip hurtSound; // drag your hurt sound here in Inspector

    private AudioSource audioSource;
    private Animator animator;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = transform.root.GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        // Auto-add AudioSource if missing
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (isPlayer && healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Play hurt sound
        if (hurtSound != null && audioSource != null)
            audioSource.PlayOneShot(hurtSound);

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
            // Reload scene — works in any scene
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        else
        {
            if (animator != null)
                animator.SetTrigger("Die");

            Destroy(gameObject, 3f);
        }
    }
}