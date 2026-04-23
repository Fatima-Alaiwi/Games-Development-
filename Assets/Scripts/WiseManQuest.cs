using UnityEngine;

/// <summary>
/// Replaces WiseMan.cs
/// Attach this to your Wise Man NPC GameObject.
///
/// SETUP IN INSPECTOR:
/// 1. Quest Settings  → drag in your "Find 3 Gold Ingots" Quest ScriptableObject
///    Make sure the Quest asset has:
///      goalItemName  = "DungeonGold"
///      goalAmount    = 3
/// 2. Key Item        → assign keyIcon sprite and collectSound clip
/// 3. Dialogue        → assign greetingClip and thankYouClip
/// 4. The GameObject needs a Collider with IsTrigger = true
/// </summary>
public class WiseManQuest : MonoBehaviour, IInteractable
{
    // ── IInteractable ────────────────────────────────────────────────────────
    [field: SerializeField]
    public string InteractionText { get; set; } = "Talk to the Wise Man";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    // ── Quest ────────────────────────────────────────────────────────────────
    [Header("Quest")]
    [Tooltip("Drag the 'Find 3 Gold Ingots' Quest ScriptableObject here.")]
    public Quest goldQuest;

    // ── Key Item ─────────────────────────────────────────────────────────────
    [Header("Key Item")]
    public Sprite  keyIcon;
    public AudioClip collectSound;

    // ── Dialogue ─────────────────────────────────────────────────────────────
    [Header("Dialogue")]
    public AudioClip greetingClip;   // "Bring me three golden ingots…"
    public AudioClip thankYouClip;   // "Well done! Take this key."

    // ── Internals ─────────────────────────────────────────────────────────────
    private bool        hasGivenKey = false;
    private AudioSource audioSource;
    private Animator    animator;

    // ═════════════════════════════════════════════════════════════════════════

    void Start()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        animator    = GetComponent<Animator>();
    }

    // ── IInteractable entry point ─────────────────────────────────────────────
    public void Interact()
    {
        if (hasGivenKey) return;               // nothing left to do

        EnsureQuestStarted();                  // give the quest on first talk

        if (QuestIsComplete())
        {
            RewardPlayer();
        }
        else
        {
            // Quest running but not yet complete – just remind the player
            PlayClip(greetingClip);
        }
    }

    // ── Trigger-based talking animation (proximity) ───────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        SetTalking(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        SetTalking(false);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // Private helpers
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Registers the quest with QuestManager the first time the player talks
    /// to the Wise Man (or re-enters trigger). Safe to call multiple times.
    /// </summary>
    void EnsureQuestStarted()
    {
        if (goldQuest == null) return;

        // AcceptQuest already guards against duplicates
        QuestManager.Instance.AcceptQuest(goldQuest);

        // Play greeting only once per "session" (quest not yet complete)
        if (!QuestIsComplete())
            PlayClip(greetingClip);

        // Update the label so the player knows what to do
        InteractionText = $"Find the {goldQuest.goalAmount} Golden Ingots";
    }

    bool QuestIsComplete()
    {
        return goldQuest != null && goldQuest.isCompleted;
    }

    void RewardPlayer()
    {
        hasGivenKey = true;

        // 1. Remove gold from inventory
        InventoryManager.instance.RemoveItem(goldQuest.goalItemName, goldQuest.goalAmount);

        // 2. Give the key
        bool added = InventoryManager.instance.AddItem("DungeonKey", keyIcon);
        if (added && collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // 3. Play thank-you voice line
        PlayClip(thankYouClip);

        // 4. Update interaction label
        InteractionText = "Safe travels, adventurer!";
        isInteractable  = false;   // optional: disable further interaction
    }

    void SetTalking(bool state)
    {
        if (animator != null)
            animator.SetBool("isTalking", state);
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