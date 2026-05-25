using UnityEngine;
using System.Collections;

public class Merchant : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to talk to Merchant";
    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Dialogue")]
    public AudioClip greetingClip;       // Plays the first time or when offering the quest
    public AudioClip repeatClip;         // Optional: Plays if spoken to again after accepting

    [Header("Peter Voice Lines")]
    // Drag Peter's reaction audio file here if needed (similar to Raghad's setup)
    public AudioClip peterReactionClip;

    [Header("Quest Requirement")]
    public Quest talkToMerchantQuest;    // Drag the "Talk to Merchant" quest here
    public Quest magesTreasureQuest;     // Drag the "Mage's Treasure" quest here

    private bool hasCompletedTalkQuest = false;
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
        if (!isInteractable) return;

        // Trigger talking animation if available
        if (animator != null)
            animator.SetBool("isTalking", true);

        // Progress Quest Logic on first interaction
        if (!hasCompletedTalkQuest)
        {
            hasCompletedTalkQuest = true;

            // 1. Complete the current objective to talk to him
            if (talkToMerchantQuest != null)
                QuestManager.Instance.UpdatedCompleteQuest(talkToMerchantQuest);

            // 2. Play the Merchant's greeting/quest offer audio
            PlayClip(greetingClip);
            Debug.Log("Merchant: You need to find the Mage's Treasure...");

            // 3. Give the player the next quest line
            if (magesTreasureQuest != null)
                QuestManager.Instance.AcceptQuest(magesTreasureQuest);

            // 4. Play Peter's voice response after a short duration so they don't overlap
            if (peterReactionClip != null && greetingClip != null)
            {
                // We add an extra 0.5f second pause so there's a natural breath between speakers
                float totalWaitTime = greetingClip.length + 0.5f;
                StartCoroutine(PlayPeterLineAfterDelay(totalWaitTime));
            }
            else if (peterReactionClip != null)
            {
                // Fallback if the greeting clip accidentally gets left unassigned in the Inspector
                StartCoroutine(PlayPeterLineAfterDelay(2f));
            }
        }
        else
        {
            // If the player talks to him again later, play standard dialogue without restarting quests
            PlayClip(repeatClip != null ? repeatClip : greetingClip);
            Debug.Log("Merchant: Go safely into the mansion, Peter.");
        }
    }

    IEnumerator PlayPeterLineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (peterReactionClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(peterReactionClip);
        }
        if (animator != null)
            animator.SetBool("isTalking", false);

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