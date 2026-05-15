using UnityEngine;

public class Brazier : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Light the brazier";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Settings")]
    public string requiredItemName = "Gas";
    public GameObject fireEffect; // drag FX_Fire prefab here
    public AudioClip lightSound;

    [Header("Gate Settings")]
    public BrazierGate gate; // reference to gate controller

    private bool isLit = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Make sure fire is off at start
        if (fireEffect != null)
            fireEffect.SetActive(false);
    }

    public void Interact()
    {
        if (isLit) return;

        // Check if player has gas
        if (!InventoryManager.instance.HasItem(requiredItemName))
        {
            InteractionText = "I need a gas bottle...";
            return;
        }

        // Use one gas bottle
        InventoryManager.instance.RemoveItem(requiredItemName, 1);

        // Light the fire
        isLit = true;
        isInteractable = false;
        InteractionText = "";

        if (fireEffect != null)
            fireEffect.SetActive(true);

        if (lightSound != null && audioSource != null)
            audioSource.PlayOneShot(lightSound);

        // Tell the gate one brazier is lit
        if (gate != null)
            gate.BrazierLit();
    }
}