using UnityEngine;
using System.Collections;

public class Mage : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to talk to the Mage";
    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Dialogue Clips")]
    public AudioClip greetingClip;       // Plays when first asking for the treasure
    public AudioClip waitClip;           // Plays if E is pressed but player lacks items
    public AudioClip rewardClip;         // Plays once Peter hands over both treasures 
    public AudioClip completedClip;      // Optional: Plays if spoken to again after finishing everything

    [Header("Peter Voice Lines")]
    public AudioClip peterClip;      // Peter's audio reaction line playing after reward

    [Header("Quest Requirements")]
    public Quest magesTreasureQuest;     // Drag the "Mage's Treasure" quest here
    public Quest returnHomeQuest;        // Optional: Next quest (e.g., escaping or final portal quest)

    [Header("Item Requirements")]
    public string itemTargetName = "TreasureItem";
    public int requiredTreasureCount = 2;

    private bool hasHandedOverTreasure = false;
    private bool playerIsNearby = false;
    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Detect pressing 'Q' to turn in items, but only if player is near the Mage and quest isn't done
        if (playerIsNearby && !hasHandedOverTreasure && Input.GetKeyDown(KeyCode.Q))
        {
            TryGiveTreasure();
        }
    }

    /// <summary>
    /// Triggered when the player presses 'E' (Standard Interaction)
    /// </summary>
    public void Interact()
    {
        if (!isInteractable) return;

        // Trigger talking animation
        SetTalkingAnimation(true);

        if (!hasHandedOverTreasure)
        {
            // Play a hint reminder telling the player what to do
            PlayClip(greetingClip);
            Debug.Log("Mage: Bring me the 2 Treasure Items, then press 'Q' to hand them over!");

            // If they don't have enough items yet, you could alternate to waitClip
            StartCoroutine(StopTalkingAfterAudio(greetingClip));
        }
        else
        {
            // Dialogue after the quest has already been finished completely
            PlayClip(completedClip != null ? completedClip : rewardClip);
            Debug.Log("Mage: Only my ancient magic can rip open the seams of reality now...");
            AudioClip played = completedClip != null ? completedClip : rewardClip;
            StartCoroutine(StopTalkingAfterAudio(played));
        }
    }

    /// <summary>
    /// Processes the item hand-over when pressing 'Q'
    /// </summary>
    private void TryGiveTreasure()
    {

        int currentCount = InventoryManager.instance.GetItemCount(itemTargetName);

        if (currentCount >= requiredTreasureCount)
        {
            hasHandedOverTreasure = true;
            SetTalkingAnimation(true);

            // 1. Deduct items from player's inventory
            InventoryManager.instance.RemoveItem(itemTargetName, requiredTreasureCount);
            Debug.Log($"Handed over {requiredTreasureCount} {itemTargetName}s successfully!");

            // 2. Complete the Mage's Treasure quest lines
            if (magesTreasureQuest != null)
                QuestManager.Instance.UpdatedCompleteQuest(magesTreasureQuest);

            // 3. Play the Mage's reward/ritual speech
            PlayClip(rewardClip);
            Debug.Log("Mage: Magnificent! The ancient magic holds power once more...");

            // 4. Begin next story quest if applicable
            if (returnHomeQuest != null)
                QuestManager.Instance.AcceptQuest(returnHomeQuest);

            // 5. Chain Peter's voice line right after the Mage finishes his line
            if (peterClip != null && rewardClip != null)
            {
                float totalWaitTime = rewardClip.length + 0.5f;
                StartCoroutine(PlayPeterLineAfterDelay(totalWaitTime));
            }
            else if (peterClip != null)
            {
                StartCoroutine(PlayPeterLineAfterDelay(2f));
            }
        }
        else
        {
            // Player pressed Q but lacks both items
            SetTalkingAnimation(true);
            PlayClip(waitClip);
            Debug.Log($"Mage: You only have {currentCount}/{requiredTreasureCount} items. I cannot help you yet.");
            StartCoroutine(StopTalkingAfterAudio(waitClip));
        }
    }

    #region Trigger Proximity Detection
    // These track if Peter is close enough to use 'Q'. Make sure the Mage has a trigger zone attached!
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
            SetTalkingAnimation(false);
        }
    }
    #endregion

    #region Helper Routines
    IEnumerator PlayPeterLineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (peterClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(peterClip);
        }
        yield return new WaitForSeconds(peterClip != null ? peterClip.length : 1f);
        SetTalkingAnimation(false);
    }

    IEnumerator StopTalkingAfterAudio(AudioClip clip)
    {
        float wait = (clip != null) ? clip.length : 2f;
        yield return new WaitForSeconds(wait);
        SetTalkingAnimation(false);
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

    void SetTalkingAnimation(bool talking)
    {
        if (animator != null)
            animator.SetBool("isTalking", talking);
    }
    #endregion
}