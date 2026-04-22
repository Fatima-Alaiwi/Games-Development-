using UnityEngine;

public class Magician : MonoBehaviour
{
    [Header("Dialogue")]
    public AudioClip greetingClip;
    public AudioClip rewardClip;

    [Header("Bottle Requirement")]
    public int requiredBottleCount = 4;

    private bool hasGivenCode = false;
    private bool hasPlayedGreeting = false;
    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (animator != null)
            animator.SetBool("isTalking", true);

        int bottleCount = GetBottleCount();

        if (!hasGivenCode && bottleCount >= requiredBottleCount)
        {
            hasGivenCode = true;
            InventoryManager.instance.RemoveItem("Bottle", 4);
            PlayClip(rewardClip);
        }
        else if (!hasPlayedGreeting && !hasGivenCode)
        {
            hasPlayedGreeting = true;
            PlayClip(greetingClip);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (animator != null)
            animator.SetBool("isTalking", false);

        hasPlayedGreeting = false;
    }

    int GetBottleCount()
    {
        foreach (var item in InventoryManager.instance.items)
        {
            if (item.itemName == "Bottle")
                return item.count;
        }
        return 0;
    }

    void PlayClip(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}