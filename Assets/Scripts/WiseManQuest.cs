using UnityEngine;

public class WiseManQuest : MonoBehaviour, IInteractable
{
    [Header("Key Item")]
    public Sprite keyIcon;
    public AudioClip collectSound;

    [Header("Dialogue")]
    public AudioClip greetingClip;
    public AudioClip thankYouClip;

    [Header("Quest")]
    public Quest goldenIngotQuest;
    public int requiredGoldCount = 3;

    [Header("Interaction")]
    [field: SerializeField]
    public string InteractionText { get; set; } = "Talk to the Wise Man";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    private bool hasGivenKey = false;
    private bool questStarted = false;
    private bool isCompletingQuest = false;
    private AudioSource audioSource;
    private Animator animator;

    void Start()
{
    audioSource = GetComponentInParent<AudioSource>();
    if (audioSource == null)
        audioSource = gameObject.AddComponent<AudioSource>();
    animator = GetComponentInParent<Animator>();

    if (goldenIngotQuest != null)
        goldenIngotQuest.ResetQuest();
}

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (animator != null)
            animator.SetBool("isTalking", false);
    }

    public void Interact()
    {
        if (hasGivenKey) return;
        if (isCompletingQuest) return; // Prevent double pressing

        int goldCount = GetGoldCount();

        if (questStarted && goldCount >= requiredGoldCount)
        {
            isCompletingQuest = true;
            StartCoroutine(CompleteQuestAndGiveKey());
        }
        else if (!questStarted)
        {
            StartGoldenIngotQuest();
        }
        else
        {
            TriggerTalkingAnimation(greetingClip);
        }
    }

    void StartGoldenIngotQuest()
    {
        questStarted = true;
        TriggerTalkingAnimation(greetingClip);

        if (goldenIngotQuest != null)
        {
            QuestManager.Instance.AcceptQuest(goldenIngotQuest);

            // If player already has gold, sync the HUD count
            int existing = GetGoldCount();
            if (existing > 0)
            {
                int safeAmount = Mathf.Min(existing, goldenIngotQuest.goalAmount);
                goldenIngotQuest.currentAmount = safeAmount;
            }
        }
        else
        {
            Debug.LogWarning("WiseManQuest: No Quest assigned in Inspector!");
        }
    }

    System.Collections.IEnumerator CompleteQuestAndGiveKey()
    {
        // Play thank you animation and audio FIRST
        TriggerTalkingAnimation(thankYouClip);

        // Wait for thank you clip to finish
        float clipLength = thankYouClip != null ? thankYouClip.length : 1f;
        yield return new WaitForSeconds(clipLength);

        // Stop talking animation
        if (animator != null)
            animator.SetBool("isTalking", false);

        // Remove gold from inventory
        InventoryManager.instance.RemoveItem("DungeonGold", requiredGoldCount);

        // Complete the quest — this removes it from HUD after 2 seconds
        if (goldenIngotQuest != null)
            QuestManager.Instance.CompleteQuestPublic(goldenIngotQuest);

       // Give the key
        bool added = InventoryManager.instance.AddItem("GoldenKey", keyIcon);
        if (added)
        {
            QuestManager.Instance.UpdateQuestCount("GoldenKey", 1);
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        hasGivenKey = true;
        InteractionText = "Thank you for your help!";
    }

    void TriggerTalkingAnimation(AudioClip clip)
    {
        if (animator != null)
            animator.SetBool("isTalking", true);

        PlayClip(clip);

        float duration = clip != null ? clip.length : 1f;
        CancelInvoke("StopTalkingAnimation");
        Invoke("StopTalkingAnimation", duration);
    }

    void StopTalkingAnimation()
    {
        if (animator != null)
            animator.SetBool("isTalking", false);
    }

    int GetGoldCount()
    {
        if (InventoryManager.instance == null) return 0;
        foreach (var item in InventoryManager.instance.items)
        {
            if (item.itemName == "DungeonGold")
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