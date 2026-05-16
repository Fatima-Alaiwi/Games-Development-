using UnityEngine;
using System.Collections;

public class HorrorMansionLockedDoor : MonoBehaviour, IInteractable
{
    [Header("Interaction Texts")]
    [SerializeField] private string textBeforeQuest = "[E] The door is locked...";
    [SerializeField] private string textNoKey       = "[E] You need a key to open this";
    [SerializeField] private string textHasKey      = "[E] Open the door";

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
    // Raghad: drag Peter_02 audio file here — plays when player presses E and door is locked
    public AudioClip peterLockedDoorClip;
    private AudioSource audioSource;

    [Header("Opening Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool questGiven = false;
    // Raghad: makes sure Peter's line only plays once
    private bool peterLineplayed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

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

        // Step 1: First interaction — door is locked, give quest, play Peter line
        if (!questGiven)
        {
            questGiven = true;

            if (doorQuest != null)
                QuestManager.Instance.AcceptQuest(doorQuest);

            UIManager.Instance.ShowHoverText("Search for the key, then come back!", transform.position);
            StartCoroutine(HideTextAfterDelay(2f));

            // Raghad: play Peter's voice line — "Locked. I need to find a key."
            if (!peterLineplayed && peterLockedDoorClip != null && audioSource != null)
            {
                peterLineplayed = true;
                audioSource.PlayOneShot(peterLockedDoorClip);
            }

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