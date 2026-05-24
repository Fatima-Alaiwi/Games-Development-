using UnityEngine;
using System.Collections;

public class LockedDoorKingdom : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Locked! Use Q to unlock with Map";
    public string InteractionText => _interactionText;
    public bool isInteractable { get; set; } = true;

    [Header("Labels")]
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Required Item Settings")]
    public string requiredMapName = "Map";

    // NEW FIELD Added to reference the lock object mesh/prefab attached to the door
    [Header("Lock Object Settings")]
    public GameObject lockObject;

    [Header("Sound")]
    public AudioClip openingDoorClip;
    public AudioClip peterReactionClip;
    public AudioClip lockedSoundClip;
    private AudioSource audioSource;

    [Header("Opening Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    // Dynamic focus tracking to prevent background inputs from a distance
    private bool isPlayerNear = false;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Standard Interact (Triggered by pressing E)
    public void Interact()
    {
        if (!isInteractable) return;

        // Play the locked sound effect to show it's locked when pressing E
        if (audioSource != null && lockedSoundClip != null)
        {
            audioSource.PlayOneShot(lockedSoundClip);
        }

        Debug.Log("This door is locked. Press 'Q' to use your Map.");
    }

    void Update()
    {
        // 1. If the door is already open, stop tracking inputs
        if (!isInteractable) return;

        // 2. CRITICAL FIX: Only check for Q if Peter is actively standing near this specific door!
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Q))
        {
            TryUnlockAndOpen();
        }
    }

    // Automatically tracks if Peter is near this exact door structure
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }

    private void TryUnlockAndOpen()
    {
        // 1. Check if the InventoryManager exists in the scene
        if (InventoryManager.instance != null)
        {
            // Tries to take the Map out of the inventory
            bool hasMap = InventoryManager.instance.RemoveItem(requiredMapName);
            if (hasMap)
            {
                Debug.Log("Map found! Opening the locked door...");
                UnlockAndOpenSequence();
            }
            else
            {
                Debug.Log("You do not have the required map: " + requiredMapName);

                // Play locked sound feedback when failing the Q check
                if (audioSource != null && lockedSoundClip != null)
                {
                    audioSource.PlayOneShot(lockedSoundClip);
                }
            }
        }
        else
        {
            // Fallback for testing if InventoryManager isn't active in your test scene
            Debug.LogWarning("InventoryManager instance missing! Force opening for testing.");
            UnlockAndOpenSequence();
        }
    }

    private void UnlockAndOpenSequence()
    {
        isInteractable = false;
        isPlayerNear = false; // Disable any further input tracking completely

        // Hides the physical lock asset from the scene right away
        if (lockObject != null)
        {
            lockObject.SetActive(false);
        }

        // Play open sound
        if (audioSource != null && openingDoorClip != null)
            audioSource.PlayOneShot(openingDoorClip);

        // Update Quest Progress if using this map item finishes a quest target
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.UpdateProgress(gameObject.name, 1);
        }

        StartCoroutine(OpenDoorAndReact());
    }

    IEnumerator OpenDoorAndReact()
    {
        // 1. Run the opening rotation on the Y axis
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

        // 2. Wait for the opening sound to finish before playing Peter's line
        if (openingDoorClip != null)
        {
            yield return new WaitForSeconds(openingDoorClip.length);
        }

        // 3. Play Peter's audio reaction line
        if (peterReactionClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(peterReactionClip);
        }
    }
}