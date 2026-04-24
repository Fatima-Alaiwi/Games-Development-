using UnityEngine;

public class PowerCore : MonoBehaviour, IInteractable
{
    public Quest powerQuest;
    public Material activeMaterial; // Drag the "On" material here
    public MeshRenderer coreRenderer; // The part of the core that changes color
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip insertSound;

    [field: SerializeField] public string InteractionText { get; set; } = "Insert Power Cell";
    public bool isInteractable { get; set; } = false; // Start locked
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void ActivateCoreInteraction()
    {
        isInteractable = true;
    }

    public void Interact()
    {
        // 1. Check if player has the cell (Check your inventory system here)
        if (InventoryManager.instance.HasItem("PowerCell")) 
        {
            InsertCell();
        }
    }

    void InsertCell()
    {
        // 2. Visual Change
        if (coreRenderer != null && activeMaterial != null)
        {
            coreRenderer.material = activeMaterial;
        }

        // 3. Sound Effect
        if (audioSource != null && insertSound != null)
        {
            audioSource.PlayOneShot(insertSound);
        }

        // 4. Update Quest
        QuestManager.Instance.UpdatedCompleteQuest(powerQuest);
        
        // 5. Cleanup
        isInteractable = false;
        InteractionText = "Core Stabilized";
        InventoryManager.instance.RemoveItem("PowerCell");
    }
}