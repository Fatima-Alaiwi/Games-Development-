using UnityEngine;
using System.Collections;

public class TwoKeyDoor : MonoBehaviour, IInteractable
{
    [Header("Required Quests")]
    public Quest requiredQuest1; // drag WiseMan quest here
    public Quest requiredQuest2; // drag DungeonLocked quest here

    [field: SerializeField]
    public string InteractionText { get; set; } = "Door is Locked";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Key Settings")]
    public string keyItemName = "GoldenKey";
    public int requiredKeyCount = 2;

    [Header("Quest")]
    public Quest keyQuest;
    public Quest dragonQuest; // Drag KillDragonQuest here

    [Header("Sound")]
    public AudioClip openingDoorClip;
    private AudioSource audioSource;

    [Header("Opening Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    private bool questStarted = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (keyQuest != null) keyQuest.ResetQuest();
        if (dragonQuest != null) dragonQuest.ResetQuest();
    }

    public void Interact()
    {
        if (isOpen) return;

        if (requiredQuest1 != null && !requiredQuest1.isCompleted)
        {
            InteractionText = "You must complete other tasks first.";
            return;
        }
        if (requiredQuest2 != null && !requiredQuest2.isCompleted)
        {
            InteractionText = "You must complete other tasks first.";
            return;
        }

        if (!questStarted)
        {
            questStarted = true;
            if (keyQuest != null)
                QuestManager.Instance.AcceptQuest(keyQuest);
        }

        int keyCount = GetKeyCount();

        if (keyQuest != null && !keyQuest.isCompleted)
            keyQuest.currentAmount = Mathf.Min(keyCount, keyQuest.goalAmount);

        if (keyCount >= requiredKeyCount)
        {
            OpenDoor();
        }
        else
        {
            InteractionText = $"Door is Locked. Keys: {keyCount}/{requiredKeyCount}";
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        isInteractable = false;
        InteractionText = "";

        InventoryManager.instance.RemoveItem(keyItemName, requiredKeyCount);

        if (keyQuest != null)
            QuestManager.Instance.CompleteQuestPublic(keyQuest);

        // Start dragon quest when door opens
        if (dragonQuest != null)
            QuestManager.Instance.AcceptQuest(dragonQuest);

        if (audioSource != null && openingDoorClip != null)
            audioSource.PlayOneShot(openingDoorClip);

        StartCoroutine(OpenDoorCoroutine());
    }

    IEnumerator OpenDoorCoroutine()
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
    }

    int GetKeyCount()
    {
        if (InventoryManager.instance == null) return 0;
        foreach (var item in InventoryManager.instance.items)
        {
            if (item.itemName == keyItemName)
                return item.count;
        }
        return 0;
    }
}