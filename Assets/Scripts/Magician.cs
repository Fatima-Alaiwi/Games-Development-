using UnityEngine;

public class Magician : MonoBehaviour
{
    [Header("Dialogue")]
    public AudioClip greetingClip;
    public AudioClip rewardClip;
    public AudioClip waitClip; // "Defeat the enemies first!" audio

    [Header("Bottle Requirement")]
    public int requiredBottleCount = 4;

    [Header("Quest Requirement")]
    public Quest killQuest; // Drag KillEnemiesQuest here

    private bool hasGivenCode = false;
    private bool hasPlayedGreeting = false;
    private bool hasPlayedWait = false;
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

        // Check if kill quest is done first
        if (killQuest != null && !killQuest.isCompleted)
        {
            // Enemies not defeated yet!
            if (!hasPlayedWait)
            {
                hasPlayedWait = true;
                PlayClip(waitClip); // Play "defeat enemies first" audio
                Debug.Log("Magician: Defeat the enemies first!");
            }
            return;
        }

        int bottleCount = GetBottleCount();

        if (!hasGivenCode && bottleCount >= requiredBottleCount)
        {
            hasGivenCode = true;
            InventoryManager.instance.RemoveItem("Bottle", 4);
            PlayClip(rewardClip);
            Debug.Log("Magician gave the password: 927!");
        }
        else if (!hasPlayedGreeting && !hasGivenCode)
        {
            hasPlayedGreeting = true;
            PlayClip(greetingClip);
            Debug.Log("Magician: Find 4 bottles!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (animator != null)
            animator.SetBool("isTalking", false);

        hasPlayedGreeting = false;
        hasPlayedWait = false;
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