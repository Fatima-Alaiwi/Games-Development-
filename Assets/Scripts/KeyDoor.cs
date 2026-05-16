using UnityEngine;
using System.Collections;
// Raghad: KeyDoor script — attach this to SM_Bld_Cell_Door_01
// Checks specifically for "HorrorKey_3" so key_1 cannot open this door
public class KeyDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Press E to Open Door";
    public string InteractionText => _interactionText;

    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Sound")]
    public AudioClip openingDoorClip;
    // Raghad: drag Peter_09 audio file here — plays when player tries to open door without key
    public AudioClip peterLockedClip;
    private AudioSource audioSource;

    [Header("Opening Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Quest")]
    // Raghad: drag FindPortalKeyQuest here in Inspector
    public Quest findPortalQuest;

    // Raghad: makes sure Peter's line only plays once
    private bool peterLinePlayed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (!isInteractable) return;

        // Only HorrorKey_3 (the library key) opens this door — key_1 will NOT work
        if (!InventoryManager.instance.HasItem("HorrorKey_3"))
        {
            _interactionText = "You need a key to open this door";

            // Raghad: play Peter's voice line once — "It's locked. There must be a key somewhere. I'll find it."
            if (!peterLinePlayed && peterLockedClip != null && audioSource != null)
            {
                peterLinePlayed = true;
                audioSource.PlayOneShot(peterLockedClip);
            }

            return;
        }

        isInteractable = false;

        // Remove key_3 from inventory
        InventoryManager.instance.RemoveItem("HorrorKey_3");

        // Complete the portal quest so HUD updates
        if (findPortalQuest != null)
            QuestManager.Instance.UpdatedCompleteQuest(findPortalQuest);

        if (audioSource != null && openingDoorClip != null)
            audioSource.PlayOneShot(openingDoorClip);

        StartCoroutine(OpenDoor());
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
    }
}