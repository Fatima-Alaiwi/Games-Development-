using UnityEngine;

public class DragonHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    public AudioClip hitSound;
    public AudioClip deathSound;

    public Quest dragonQuest; // Drag KillDragonQuest here

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
    if (currentHealth <= 0) return; 
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

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.ResetTrigger("hurt");
            anim.CrossFadeInFixedTime("Hurt", 0.15f, 0, 0f);
        }
    }
}

    void Die()
    {
        Debug.Log("Dragon is dead!");

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        if (deathSound != null)
        {
            audioSource.volume = 1f;
            audioSource.spatialBlend = 0f;
            audioSource.PlayOneShot(deathSound);
        }

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetTrigger("Die");

        // Complete the dragon quest
        if (dragonQuest != null)
            QuestManager.Instance.CompleteQuestPublic(dragonQuest);

        Destroy(gameObject, 3f);
    }
}