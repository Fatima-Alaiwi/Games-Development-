using UnityEngine;
using System.Collections;

public class HorrorMansionLockedDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Press E to Open Door";
    public string InteractionText => _interactionText;

    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Key")]
    public string requiredKeyName = "HorrorKey";

    [Header("Sound")]
    public AudioClip openingDoorClip;
    public AudioClip magicianCallClip;
    private AudioSource audioSource;

    [Header("Opening Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (!isInteractable) return;

        if (!HasKey())
        {
            UIManager.Instance.ShowHoverText("Search for the key, then press E to open the door!", transform.position);
            StartCoroutine(HideTextAfterDelay(2f));
            return;
        }

        InventoryManager.instance.RemoveItem(requiredKeyName, 1);
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

        // Play magician call once after door opens
        if (magicianCallClip != null)
            AudioSource.PlayClipAtPoint(magicianCallClip, transform.position);
    }
}