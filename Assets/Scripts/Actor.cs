using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Actor : MonoBehaviour
{
    int currentHealth;
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

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hurtSound != null && audioSource != null)
            audioSource.PlayOneShot(hurtSound);

        if (isPlayer && healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Death();
    }