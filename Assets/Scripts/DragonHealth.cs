using UnityEngine;
 
public class DragonHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;
 
    public AudioClip hitSound;
    public AudioClip deathSound;
 
    private AudioSource audioSource;
 
    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
 
    public void TakeBombDamage()
{
    currentHealth--;
    Debug.Log("Dragon hit! Health remaining: " + currentHealth);

    if (currentHealth <= 0)
    {
        Die();
    }
    else
    {
        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);
    }
}
 
  void Die()
{
    Debug.Log("Dragon is dead!");

    // Disable colliders so bombs don't hit it anymore
    foreach (Collider c in GetComponentsInChildren<Collider>())
        c.enabled = false;

    // Play death sound
    if (deathSound != null)
    {
        audioSource.volume = 1f;
        audioSource.spatialBlend = 0f;
        audioSource.PlayOneShot(deathSound);
    }

    // Trigger death animation
    Animator anim = GetComponentInChildren<Animator>();
    if (anim != null)
        anim.SetTrigger("Die");

    // Destroy after animation plays (adjust delay if needed)
    Destroy(gameObject, 3f);
}
}