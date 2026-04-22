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

    // Hide the dragon immediately
    foreach (Renderer r in GetComponentsInChildren<Renderer>())
        r.enabled = false;

    // Disable colliders immediately
    foreach (Collider c in GetComponentsInChildren<Collider>())
        c.enabled = false;

    if (deathSound != null)
    {
        audioSource.volume = 1f;
        audioSource.spatialBlend = 0f;
        audioSource.PlayOneShot(deathSound);
    }

    Destroy(gameObject, deathSound != null ? deathSound.length : 0f);
}
}