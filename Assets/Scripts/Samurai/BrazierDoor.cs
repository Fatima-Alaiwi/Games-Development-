using UnityEngine;
using System.Collections;

public class BrazierDoor : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string _interactionText = "The door is sealed...";
    public string InteractionText => _interactionText;
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;
    [Header("Spawner")]
    public EnemySpawner brazierSpawner;
    [Header("Requirements")]
    public BrazierGate brazierGate;
    public string requiredKeyName = "Key1"; // must match inventory item name

    [Header("Quest")]
    public Quest farmerQuest; // drag your farmer quest here in Inspector

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

    void Update()
    {
        // Dynamically update interaction text so player knows what's missing
        if (isOpen) return;

        bool braziersLit = brazierGate != null && brazierGate.BothLit();
        bool hasKey = InventoryManager.instance != null && 
                      InventoryManager.instance.HasItem(requiredKeyName);

        if (!braziersLit && !hasKey)
            _interactionText = "The door won't budge... light the braziers and find a key.";
        else if (!braziersLit)
            _interactionText = "The braziers must be lit first...";
        else if (!hasKey)
            _interactionText = "You need a key to open this gate.";
        else
            _interactionText = "Unlock the gate [E]";
    }

    public void Interact()
    {
        if (isOpen) return;

        bool braziersLit = brazierGate != null && brazierGate.BothLit();
        bool hasKey = InventoryManager.instance != null && 
                      InventoryManager.instance.HasItem(requiredKeyName);

        if (!braziersLit)
        {
            Debug.Log("Braziers not lit yet.");
            return;
        }

        if (!hasKey)
        {
            Debug.Log("No key in inventory.");
            return;
        }

        // Consume the key
        InventoryManager.instance.RemoveItem(requiredKeyName, 1);

        // Play gate sound via BrazierGate
        if (brazierGate != null)
            brazierGate.PlayOpenSound();

        OpenDoor();
    }

    void OpenDoor()
    {
        isOpen = true;
        isInteractable = false;
        _interactionText = "The gate is open!";

        if (openingDoorClip != null && audioSource != null)
            audioSource.PlayOneShot(openingDoorClip);
        if (brazierSpawner != null)
        brazierSpawner.StartSpawning();

        StartCoroutine(OpenDoorCoroutine());
    }

    IEnumerator OpenDoorCoroutine()
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