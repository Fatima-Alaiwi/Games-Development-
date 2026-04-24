using UnityEngine;

public class Magician : MonoBehaviour
{
    [Header("Dialogue")]
    public AudioClip greetingClip;
    public AudioClip rewardClip;
    public AudioClip waitClip;

    [Header("Bottle Requirement")]
    public int requiredBottleCount = 4;

    [Header("Quest Requirement")]
    public Quest killQuest;
    public Quest findMagicianQuest; // drag FindMagicianQuest here
    public Quest findLibraryQuest;  // drag FindLibraryQuest here

    private bool hasGivenCode = false;
    private bool hasPlayedGreeting = false;
    private bool hasPlayedWait = false;
    private bool hasCompletedFindQuest = false;
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

        // Check kill quest first
        if (killQuest != null && !killQuest.isCompleted)
        {
            if (!hasPlayedWait)
            {
                hasPlayedWait = true;
                PlayClip(waitClip);
                Debug.Log("Magician: Defeat the enemies first!");
            }
            return;
        }

        // Complete find magician quest
        if (!hasCompletedFindQuest)
        {
            hasCompletedFindQuest = true;
            if (findMagicianQuest != null)
                QuestManager.Instance.UpdatedCompleteQuest(findMagicianQuest);
            Debug.Log("Find Magician quest completed! Bottles unlocked!");
        }

        int bottleCount = GetBottleCount();
        Debug.Log("Bottle count: " + bottleCount); // so you can see in console

        // CHECK BOTTLES EVERY TIME — not just first visit
        if (!hasGivenCode && bottleCount >= requiredBottleCount)
        {
            hasGivenCode = true;
            InventoryManager.instance.RemoveItem("Bottle", 4);
            PlayClip(rewardClip);

            // Start library quest — player must find the library door
            // Code is NOT shown on screen — player must remember what Magician said!
            if (findLibraryQuest != null)
                QuestManager.Instance.AcceptQuest(findLibraryQuest);

            Debug.Log("Magician gave the password: 927!");
        }
        else if (!hasGivenCode)
        {
            // Play greeting every time until bottles collected
            PlayClip(greetingClip);
            Debug.Log("Magician: Find 4 bottles! You have: " + bottleCount);
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