using UnityEngine;
using System.Collections;

public class LockedBox : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Locked! Use Q to unlock with Key";
    public string InteractionText => _interactionText;
    public bool isInteractable { get; set; } = true;

    [Header("Labels")]
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Key Settings")]
    public string requiredKeyName = "Key";

    [Header("Sound")]
    public AudioClip openingBoxClip;
    public AudioClip peterReactionClip;
    public AudioClip lockedSoundClip;
    private AudioSource audioSource;

    [Header("Opening Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Glow Settings")]
    public Light glowLight;

    // Dynamic focus tracking to prevent background inputs
    private bool isPlayerNear = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Standard Interact (Triggered by pressing E or looking at the object)
    public void Interact()
    {
        if (!isInteractable) return;

        if (audioSource != null && lockedSoundClip != null)
        {
            audioSource.PlayOneShot(lockedSoundClip);
        }

        Debug.Log("This box is locked. Press 'Q' to use your Key.");
    }

    void Update()
    {
        // 1. If the box is already open, stop tracking inputs
        if (!isInteractable) return;

        // 2. CRITICAL FIX: Only check for Q if Peter is actively standing near this specific box!
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Q))
        {
            TryUnlockAndOpen();
        }
    }

    // Automatically tracks if Peter is near this exact box
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
        if (InventoryManager.instance != null)
        {
            // This will now only execute if the player is standing next to the box
            bool hasKey = InventoryManager.instance.RemoveItem(requiredKeyName);
            if (hasKey)
            {
                Debug.Log("Key found! Opening the locked box...");
                UnlockAndOpenSequence();
            }
            else
            {
                Debug.Log("You do not have the required key: " + requiredKeyName);

                if (audioSource != null && lockedSoundClip != null)
                {
                    audioSource.PlayOneShot(lockedSoundClip);
                }
            }
        }
        else
        {
            Debug.LogWarning("InventoryManager instance missing! Force opening for testing.");
            UnlockAndOpenSequence();
        }
    }

    private void UnlockAndOpenSequence()
    {
        isInteractable = false;
        isPlayerNear = false; // Disable any further input tracking completely

        if (glowLight != null) glowLight.enabled = false;

        if (audioSource != null && openingBoxClip != null)
            audioSource.PlayOneShot(openingBoxClip);

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.UpdateProgress(gameObject.name, 1);
        }

        StartCoroutine(OpenBoxAndReact());
    }

    IEnumerator OpenBoxAndReact()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(openAngle, 0, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }
        transform.rotation = endRotation;

        if (openingBoxClip != null)
        {
            yield return new WaitForSeconds(openingBoxClip.length);
        }

        if (peterReactionClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(peterReactionClip);
        }
    }
}