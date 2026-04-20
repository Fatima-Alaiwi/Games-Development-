using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Press E to Open Door";
    public string InteractionText => _interactionText;

    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Sound")]
    public AudioClip openingDoorClip;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (!isInteractable) return;

        transform.Rotate(0, 90, 0);
        isInteractable = false;

        if (audioSource != null && openingDoorClip != null)
            audioSource.PlayOneShot(openingDoorClip);
    }
}