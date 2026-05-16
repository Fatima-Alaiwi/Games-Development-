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

    public AudioClip fabricSound;
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


        // Wait a moment then drop the fabric
        yield return new WaitForSeconds(0.5f);
        DropFabric();
    }

    void DropFabric()
{
    if (fabric != null)
        StartCoroutine(SlideFabricDown());
}

IEnumerator SlideFabricDown()
{
    Vector3 startPos = fabric.transform.position;
    Vector3 endPos = startPos + new Vector3(-5f, 0, -1f);
    float duration = 1.5f;
    float t = 0f;
    bool soundPlayed = false;

    while (t < 1f)
    {
        t += Time.deltaTime / duration;

        // Play sound after 1 second of sliding (when t passes 1/1.5 ≈ 0.667)
        if (!soundPlayed && t >= (1f / duration))
        {
            soundPlayed = true;
            if (fabricSound != null)
                audioSource.PlayOneShot(fabricSound);
        }

        fabric.transform.position = Vector3.Lerp(startPos, endPos, t);
        yield return null;
    }
}
}