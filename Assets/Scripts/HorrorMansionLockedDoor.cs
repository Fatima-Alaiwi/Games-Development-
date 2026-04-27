using UnityEngine;
using System.Collections;

public class HorrorMansionLockedDoor : MonoBehaviour, IInteractable
{
    // TEXT STATES - edit these in the Inspector
    [Header("Interaction Texts")]
    [SerializeField] private string textBeforeQuest = "[E] The door is locked...";
    [SerializeField] private string textNoKey       = "[E] You need a key to open this";
    [SerializeField] private string textHasKey      = "[E] Open the door";

    // This is what the interaction system reads every frame
    public string InteractionText => GetInteractionText();

    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Quest")]
    public Quest doorQuest;

    [Header("Key")]
    public string requiredKeyName = "HorrorKey";

    [Header("Sound")]
    public AudioClip openingDoorClip;
    public AudioClip magicianCallClip;
    private AudioSource audioSource;

    [Header("Opening Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool questGiven = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Dynamically returns the correct label depending on state
    private string GetInteractionText()
    {
        if (!questGiven)
            return textBeforeQuest;

        if (HasKey())
            return textHasKey;

        return textNoKey;
    }

    public void Interact()
    {
        if (!isInteractable) return;

        // Step 1: First interaction — give the quest
        if (!questGiven)
        {
            questGiven = true;

            if (doorQuest != null)
                QuestManager.Instance.AcceptQuest(doorQuest);

            UIManager.Instance.ShowHoverText("Search for the key, then come back!", transform.position);
            StartCoroutine(HideTextAfterDelay(2f));
            return;
        }

        // Step 2: No key yet
        if (!HasKey())
        {
            UIManager.Instance.ShowHoverText("You don't have the key yet!", transform.position);
            StartCoroutine(HideTextAfterDelay(2f));
            return;
        }

        // Step 3: Has key — open it!
        InventoryManager.instance.RemoveItem(requiredKeyName, 1);

        if (doorQuest != null)
            QuestManager.Instance.UpdateProgress(doorQuest.goalItemName, 1);

        isInteractable = false;

        if (audioSource != null && openingDoorClip != null)
            audioSource.PlayOneShot(openingDoorClip);

        StartCoroutine(OpenDoor());
    }

    bool HasKey()
    {
        foreach (var item in InventoryManager.instance.items)
        {
            if (item.itemName == requiredKeyName)
                return true;
        }
        return false;
    }

    IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UIManager.Instance.HideHoverText();
    }

    IEnumerator OpenDoor()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, openAngle, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;

        if (magicianCallClip != null)
            AudioSource.PlayClipAtPoint(magicianCallClip, transform.position);
    }
}