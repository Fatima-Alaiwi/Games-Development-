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
    public AudioClip peterLockedDoorClip;
    private AudioSource audioSource;

    [Header("Opening Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    // Raghad: drag the EnemySpawner object here — spawns Yokai when player enters mansion
    [Header("Yokai Spawner")]
    public EnemySpawner yokaiSpawner;

    private bool questGiven = false;
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

        // Step 1: First interaction — if player already has the key, skip locked dialogue and open directly
        if (!questGiven)
        {
            questGiven = true;

            if (doorQuest != null)
                QuestManager.Instance.AcceptQuest(doorQuest);

            // Only play locked sound and show locked message if player does NOT have the key
            if (!HasKey())
            {
                UIManager.Instance.ShowHoverText("Search for the key, then come back!", transform.position);
                StartCoroutine(HideTextAfterDelay(2f));

                if (!peterLineplayed && peterLockedDoorClip != null && audioSource != null)
                {
                    peterLineplayed = true;
                    audioSource.PlayOneShot(peterLockedDoorClip);
                }

                return;
            }
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

        // Raghad: start spawning Yokai when player opens the mansion door
        if (yokaiSpawner != null)
            yokaiSpawner.StartSpawning();

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