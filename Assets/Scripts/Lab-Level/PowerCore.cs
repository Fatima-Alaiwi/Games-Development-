using UnityEngine;

public class PowerCore : MonoBehaviour, IInteractable
{
    public Quest powerQuest;
    public Material activeMaterial; 
    public MeshRenderer coreRenderer; 
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip insertSound;

    [field: SerializeField] public string InteractionText { get; set; } = "Press [E] to Insert Power Cell";
    public bool isInteractable { get; set; } = false;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void ActivateCoreInteraction()
    {
        isInteractable = true;
    }

    public void Interact()
    {
        // Only allow interaction if the Panel has activated it
        if (!isInteractable) return;

        // Check inventory for the specific item name
        if (InventoryManager.instance.HasItem("PowerCell")) 
        {
            InsertCell();
        }
        else
        {
            // Tell the player they are missing the item
            InteractionText = "Requires Power Cell";
            Debug.Log("You don't have the Power Cell yet!");
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