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
    public GameObject fireEffect;
    public AudioClip lightSound;

    [Header("Voice Line")]
    public AudioClip voiceLine; // drag voice clip here in Inspector

    [Header("Gate Settings")]
    public BrazierGate gate;

    private bool isLit = false;
    private AudioSource audioSource;

    void Update()
    {
        if (isLit) return;

        bool hasGas = InventoryManager.instance != null &&
                      InventoryManager.instance.HasItem(requiredItemName);

        InteractionText = hasGas ? "Light the brazier [E]" : "Get the gas from the Zen Garden";
        isInteractable = true;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (fireEffect != null)
            fireEffect.SetActive(false);
    }

    public void Interact()
    {
        if (isLit) return;

        if (!InventoryManager.instance.HasItem(requiredItemName))
            return;

        InventoryManager.instance.RemoveItem(requiredItemName, 1);

        isLit = true;
        isInteractable = false;
        InteractionText = "";

        if (fireEffect != null)
            fireEffect.SetActive(true);

        if (lightSound != null && audioSource != null)
            audioSource.PlayOneShot(lightSound);

        // Play voice line on VoiceAudioSource so it doesn't
        // conflict with the light sound on this object
        if (voiceLine != null)
        {
            GameObject voiceObj = GameObject.Find("VoiceAudioSource");
            if (voiceObj != null)
            {
                AudioSource voiceSource = voiceObj.GetComponent<AudioSource>();
                if (voiceSource != null)
                    voiceSource.PlayOneShot(voiceLine);
            }
        }

        if (gate != null)
            gate.BrazierLit();
    }
}