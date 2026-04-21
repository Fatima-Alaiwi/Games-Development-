using UnityEngine;
using System.Collections;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Press E to Pull Lever";
    public string InteractionText => _interactionText;
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Lever Settings")]
    public float targetZRotation = -60f;   // how far it rotates down
    public float rotateSpeed = 2f;

    [Header("Fabric")]
    public GameObject fabric;              // drag SM_Env_SpiderWeb_03 here

    [Header("Sound")]
    public AudioClip leverSound;
    private AudioSource audioSource;

    private bool isActivated = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Interact()
    {
        if (!isInteractable || isActivated) return;

        isActivated = true;
        isInteractable = false;

        if (leverSound != null)
            audioSource.PlayOneShot(leverSound);

        StartCoroutine(PullLever());
    }

    IEnumerator PullLever()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(targetZRotation, 0, 0);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotateSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;

        // Wait a moment then drop the fabric
        yield return new WaitForSeconds(0.5f);
        DropFabric();
    }

    void DropFabric()
    {
        if (fabric != null)
        {
            Rigidbody rb = fabric.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.linearDamping = 0.5f;  // slight drag so it falls naturally
        }
    }
}