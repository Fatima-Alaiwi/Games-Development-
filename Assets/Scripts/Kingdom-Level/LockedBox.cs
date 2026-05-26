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
    public Transform lidTransform;

    [Header("Glow Settings")]
    public Light glowLight;
    [Header("Contents")]
    public GameObject chestContents; // drag Map object here

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (!isInteractable) return;

        if (audioSource != null && lockedSoundClip != null)
            audioSource.PlayOneShot(lockedSoundClip);

        Debug.Log("This box is locked. Press 'Q' to use your Key.");
    }

    void Update()
    {
        if (!isInteractable) return;

        if (Input.GetKeyDown(KeyCode.Q))
            TryUnlockAndOpen();
    }

    private void TryUnlockAndOpen()
    {
        if (InventoryManager.instance != null)
        {
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
                    audioSource.PlayOneShot(lockedSoundClip);
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

        // Disable this collider so it no longer blocks the map
        GetComponent<Collider>().enabled = false;

        if (glowLight != null) glowLight.enabled = false;

        if (audioSource != null && openingBoxClip != null)
            audioSource.PlayOneShot(openingBoxClip);

        if (QuestManager.Instance != null)
            QuestManager.Instance.UpdateProgress(gameObject.name, 1);

        StartCoroutine(OpenBoxAndReact());
    }

    IEnumerator OpenBoxAndReact()
    {
        Transform t = lidTransform != null ? lidTransform : transform;
        Quaternion startRotation = t.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(openAngle, 0, 0);

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * openSpeed;
            t.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed);
            yield return null;
        }
        t.rotation = endRotation;

        if (openingBoxClip != null)
            yield return new WaitForSeconds(openingBoxClip.length);

        // Reveal contents after lid opens
        if (chestContents != null)
        {
            chestContents.transform.SetParent(null); // detach from chest
            chestContents.SetActive(true);
        }

        if (peterReactionClip != null && audioSource != null)
            audioSource.PlayOneShot(peterReactionClip);
    }
}