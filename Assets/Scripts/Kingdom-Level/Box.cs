using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Press E to Open Box";
    public string InteractionText => _interactionText;
    public bool isInteractable { get; set; } = true;

    [Header("Labels")]
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Sound")]
    public AudioClip openingBoxClip;
    // New: Field for Peter's reaction
    public AudioClip peterReactionClip;
    private AudioSource audioSource;

    [Header("Opening Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Glow Settings")]
    public Light glowLight; // Drag the Point Light here in Inspector



    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void Interact()
    {
        if (!isInteractable) return;
        isInteractable = false;

        // Turn off the glow when interacted with
        if (glowLight != null) glowLight.enabled = false;

        if (audioSource != null && openingBoxClip != null)
            audioSource.PlayOneShot(openingBoxClip);

        StartCoroutine(OpenBoxAndReact());
    }

    IEnumerator OpenBoxAndReact()
    {
        // 1. Run the opening animation
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

        // 2. Wait for the opening sound to finish before playing Peter's line
        if (openingBoxClip != null)
        {
            yield return new WaitForSeconds(openingBoxClip.length);
        }

        // 3. Play Peter's reaction
        if (peterReactionClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(peterReactionClip);
        }
    }
}