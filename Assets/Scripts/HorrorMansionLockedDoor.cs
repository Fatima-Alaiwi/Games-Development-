using UnityEngine;
using System.Collections;

public class HorrorMansionLockedDoor : MonoBehaviour, IInteractable
{
    [Header("Interaction Texts")]
    [SerializeField] private string textBeforeQuest = "[E] The door is locked...";
    [SerializeField] private string textNoKey = "[E] You need a key to open this";
    [SerializeField] private string textHasKey = "[E] Open the door";

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
    public AudioClip peterEnteringClip;
    private AudioSource audioSource;

    [Header("Opening Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Second Door")]
    public Transform secondDoor;

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

        if (!questGiven)
        {
            questGiven = true;

            if (doorQuest != null)
                QuestManager.Instance.AcceptQuest(doorQuest);

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

        if (!HasKey())
        {
            UIManager.Instance.ShowHoverText("You don't have the key yet!", transform.position);
            StartCoroutine(HideTextAfterDelay(2f));
            return;
        }

        InventoryManager.instance.RemoveItem(requiredKeyName, 1);

        if (doorQuest != null)
            QuestManager.Instance.UpdateProgress(doorQuest.goalItemName, 1);

        isInteractable = false;

        if (audioSource != null && openingDoorClip != null)
            audioSource.PlayOneShot(openingDoorClip);

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
        Quaternion firstStartRotation = transform.rotation;
        Quaternion firstEndRotation = firstStartRotation * Quaternion.Euler(0, openAngle, 0);

        Quaternion secondStartRotation = Quaternion.identity;
        Quaternion secondEndRotation = Quaternion.identity;

        if (secondDoor != null)
        {
            secondStartRotation = secondDoor.rotation;
            secondEndRotation = secondStartRotation * Quaternion.Euler(0, -openAngle, 0);
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;

            transform.rotation = Quaternion.Lerp(firstStartRotation, firstEndRotation, t);

            if (secondDoor != null)
                secondDoor.rotation = Quaternion.Lerp(secondStartRotation, secondEndRotation, t);

            yield return null;
        }

        transform.rotation = firstEndRotation;

        if (secondDoor != null)
            secondDoor.rotation = secondEndRotation;

        yield return new WaitForSeconds(1f);

        if (peterEnteringClip != null && audioSource != null)
            audioSource.PlayOneShot(peterEnteringClip);

        yield return new WaitForSeconds(2f);

        if (magicianCallClip != null)
            AudioSource.PlayClipAtPoint(magicianCallClip, transform.position);
    }
}