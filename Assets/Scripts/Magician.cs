using UnityEngine;
using System.Collections;

public class Magician : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to talk to Magician";
    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Dialogue")]
    public AudioClip greetingClip;
    public AudioClip rewardClip;
    public AudioClip waitClip;

    [Header("Peter Voice Lines")]
    // Raghad: drag Peter_10 audio file here — plays after Magician gives the code
    public AudioClip peterCodeClip;

    [Header("Bottle Requirement")]
    public int requiredBottleCount = 4;

    [Header("Quest Requirement")]
    public Quest killQuest;
    public Quest findMagicianQuest;
    public Quest findLibraryQuest;
    public Quest bottleQuest; // Drag BottleQuest here in Inspector

    private bool hasGivenCode = false;
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

    public void Interact()
    {
        if (animator != null)
            animator.SetBool("isTalking", true);

        if (killQuest != null && !QuestManager.Instance.IsQuestComplete(killQuest))
        {
            if (!hasPlayedWait)
            {
                hasPlayedWait = true;
                PlayClip(waitClip);
                Debug.Log("Magician: Defeat the enemies first!");
            }
            return;
        }

        if (!hasCompletedFindQuest)
        {
            hasCompletedFindQuest = true;

            if (findMagicianQuest != null)
                QuestManager.Instance.UpdatedCompleteQuest(findMagicianQuest);

            // Start bottle quest
            if (bottleQuest != null)
                QuestManager.Instance.AcceptQuest(bottleQuest);

            // Count bottles already in inventory and update quest progress immediately
            int alreadyCollected = GetBottleCount();
            if (alreadyCollected > 0 && bottleQuest != null)
            {
                QuestManager.Instance.UpdateProgress("Bottle", alreadyCollected);
                Debug.Log("Player already had " + alreadyCollected + " bottles — quest updated!");
            }

            Debug.Log("Find Magician quest completed! Bottles unlocked!");
        }

        int bottleCount = GetBottleCount();
        Debug.Log("Bottle count: " + bottleCount);

        if (!hasGivenCode && bottleCount >= requiredBottleCount)
        {
            hasGivenCode = true;
            InventoryManager.instance.RemoveItem("Bottle", 4);
            PlayClip(rewardClip);

            // Start library quest
            if (findLibraryQuest != null)
                QuestManager.Instance.AcceptQuest(findLibraryQuest);

            // Raghad: after Magician finishes giving the code, play Peter's reaction
            // "A code. I need to remember this. The library... I have to find the library."
            StartCoroutine(PlayPeterCodeLineAfterDelay(12f)); // change this number

            Debug.Log("Magician gave the password: 927!");
        }
        else if (!hasGivenCode)
        {
            PlayClip(greetingClip);
            Debug.Log("Magician: Find 4 bottles! You have: " + bottleCount);
        }
    }

    IEnumerator PlayPeterCodeLineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Raghad: plays Peter's line after Magician finishes talking — no overlap
        if (peterCodeClip != null && audioSource != null)
            audioSource.PlayOneShot(peterCodeClip);
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