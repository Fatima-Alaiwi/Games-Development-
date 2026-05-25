using UnityEngine;
using System.Collections;

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
        isInteractable = false;

        GetComponent<SaveableDoor>()?.MarkOpened();

        if (audioSource != null && openingDoorClip != null)
            audioSource.PlayOneShot(openingDoorClip);

        StartCoroutine(OpenDoor());
    }

    public void SnapOpen()
    {
        isInteractable = false;
        transform.rotation = transform.rotation * Quaternion.Euler(0, openAngle, 0);
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