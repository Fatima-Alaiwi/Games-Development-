using UnityEngine;
using System.Collections;

public class Magician : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to talk to Magician";
    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Dialogue — Magician Voice Lines")]
    public AudioClip greetingClip;       // E press #1 — tells Peter to find 4 bottles
    public AudioClip notAllBottlesClip;  // E press #2+ while bottles < 4
    public AudioClip rewardClip;         // Plays when player has 4 bottles — gives password
    public AudioClip waitClip;           // Kill quest not done yet

    [Header("Dialogue — Peter Voice Lines")]
    public AudioClip peterCodeClip;      // Peter reacts after hearing the password

    [Header("Bottle Requirement")]
    public int requiredBottleCount = 4;

    [Header("Quest References")]
    public Quest killQuest;
    public Quest findMagicianQuest;
    public Quest findLibraryQuest;
    public Quest bottleQuest;

    [Header("Library Key")]
    [Tooltip("Collectible ID of the library key (horror_key2). No replay if player already has it.")]
    public string libraryKeyCollectibleId = "horr_key2";

    // Internal state
    private bool hasGivenCode = false;
    private bool hasPlayedWait = false;
    private bool hasCompletedFindQuest = false;
    private bool hasPlayedGreeting = false; // true after E press #1
    private bool stateRestoredFromSave = false;
    private bool hasReplayedCode = false; // one reminder replay per session after load

    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        animator = GetComponent<Animator>();
    }

    // Restores runtime flags from saved quest data.
    // Called lazily on first Interact() so save data is guaranteed to be loaded by then.
    void RestoreStateFromSave()
    {
        if (QuestManager.Instance == null) return;

        // findLibraryQuest active or completed is the ONLY reliable indicator that the code was given.
        // bottleQuest completes when bottles are COLLECTED, not when given to the magician,
        // so it must NOT be used here — it would skip RemoveItem and leave bottles in inventory.
        if (findLibraryQuest != null &&
            (QuestManager.Instance.IsQuestComplete(findLibraryQuest) ||
             QuestManager.Instance.activeQuests.Contains(findLibraryQuest)))
        {
            hasGivenCode = true;
            hasPlayedGreeting = true;
            hasCompletedFindQuest = true;
            return;
        }

        // findMagicianQuest completed means greeting was played but code not given yet
        if (findMagicianQuest != null && QuestManager.Instance.IsQuestComplete(findMagicianQuest))
        {
            hasPlayedGreeting = true;
            hasCompletedFindQuest = true;
        }
    }

    public void Interact()
    {
        if (!stateRestoredFromSave)
        {
            stateRestoredFromSave = true;
            RestoreStateFromSave();
        }

        if (animator != null)
            animator.SetBool("isTalking", true);

        // --- BLOCK: Kill quest must be done first ---
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

        // --- Already gave the code: replay password once as a reminder, then do nothing ---
        if (hasGivenCode)
        {
            if (!hasReplayedCode)
            {
                hasReplayedCode = true;

                // If player already opened the library (has the key or completed find library quest),
                // they don't need to hear the password again — skip the replay entirely
                bool alreadyHasLibraryKey = CollectibleTracker.IsCollected(libraryKeyCollectibleId);
                bool libraryQuestDone = findLibraryQuest != null &&
                                        QuestManager.Instance.IsQuestComplete(findLibraryQuest);

                if (!alreadyHasLibraryKey && !libraryQuestDone)
                {
                    PlayClip(rewardClip);
                    float replayDelay = rewardClip != null ? rewardClip.length + 0.3f : 10f;
                    StartCoroutine(PlayPeterLineAfterDelay(replayDelay));
                }
            }
            return;
        }

        // --- E press #1: Play greeting, complete find quest, start bottle quest ---
        if (!hasPlayedGreeting)
        {
            hasPlayedGreeting = true;

            // Complete find magician quest
            if (!hasCompletedFindQuest)
            {
                hasCompletedFindQuest = true;

                if (findMagicianQuest != null)
                    QuestManager.Instance.UpdatedCompleteQuest(findMagicianQuest);

                if (bottleQuest != null)
                    QuestManager.Instance.AcceptQuest(bottleQuest);

                // If player already had some bottles, update quest progress
                int alreadyCollected = GetBottleCount();
                if (alreadyCollected > 0 && bottleQuest != null)
                {
                    QuestManager.Instance.UpdateProgress("Bottle", alreadyCollected);
                    Debug.Log("Player already had " + alreadyCollected + " bottles — quest updated!");
                }
            }

            PlayClip(greetingClip);
            Debug.Log("Magician: Greeting played — go find the 4 bottles!");
            return;
        }

        // --- E press #2 onwards ---
        int bottleCount = GetBottleCount();
        Debug.Log("Bottle count: " + bottleCount);

        // Player does not have all 4 bottles yet
        if (bottleCount < requiredBottleCount)
        {
            PlayClip(notAllBottlesClip);
            Debug.Log("Magician: Not enough bottles! You have: " + bottleCount);
            return;
        }

        // Player has all 4 bottles — give the password
        hasGivenCode = true;
        hasReplayedCode = true; // block any accidental second E press during playback
        isInteractable = false;
        InventoryManager.instance.RemoveItem("Bottle", 4);

        if (findLibraryQuest != null)
            QuestManager.Instance.AcceptQuest(findLibraryQuest);

        if (bottleQuest != null)
            QuestManager.Instance.UpdatedCompleteQuest(bottleQuest);

        PlayClip(rewardClip);
        Debug.Log("Magician gave the password: 927!");

        // Peter reacts automatically after the reward clip finishes
        float delay = rewardClip != null ? rewardClip.length + 0.3f : 10f;
        StartCoroutine(PlayPeterLineAfterDelay(delay));
    }

    IEnumerator PlayPeterLineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (peterCodeClip != null && audioSource != null)
            audioSource.PlayOneShot(peterCodeClip);
        Debug.Log("Peter reacts to the password.");

        // Re-enable interaction after the full sequence so player can ask for reminder later
        float peterLineDuration = peterCodeClip != null ? peterCodeClip.length + 0.3f : 2f;
        yield return new WaitForSeconds(peterLineDuration);
        isInteractable = true;
        hasReplayedCode = false; // allow one reminder replay now that playback is fully done
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