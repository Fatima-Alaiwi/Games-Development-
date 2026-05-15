using UnityEngine;

public class BrazierDoor : MonoBehaviour
{
    
    [Header("Interaction")]
    [SerializeField] private string _interactionText = "The door is sealed...";
    public string InteractionText => _interactionText;
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Requirements")]
    public BrazierGate brazierGate;

    [Header("Door Settings")]
    public float openAngle = -90f;
    public float openSpeed = 2f;

    [Header("Sound")]
    public AudioClip openingDoorClip;
    private AudioSource audioSource;

    private bool isOpen = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Interact()
    {
        if (isOpen) return;

        if (brazierGate != null && !brazierGate.BothLit())
        {
            _interactionText = "The door won't budge... perhaps fire holds the answer.";
            return;
        }

        OpenDoor();
    }

    public void OpenFromBrazier()
    {
        if (isOpen) return;
        OpenDoor();
    }

    void OpenDoor()
    {
        isOpen = true;
        isInteractable = false;
        _interactionText = "The door is open!";

        if (openingDoorClip != null && audioSource != null)
            audioSource.PlayOneShot(openingDoorClip);

        StartCoroutine(OpenDoorCoroutine());
    }

    System.Collections.IEnumerator OpenDoorCoroutine()
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
