using UnityEngine;

public class KeycardItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public string itemName = "Keycard";
    public Sprite itemIcon;
    public AudioClip collectSound;

    [Header("Quest Data")]
    public string questName = "Portal Access";
    public string nextStepDescription = "Unlock the portal room";
    public Quest portalQuestAsset;

    [Header("Interaction Settings")]
    [field: SerializeField] public string InteractionText { get; set; } = "Press E to pick up Keycard";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            // Tell the manager we found the item so the 0/1 becomes 1/1
            QuestManager.Instance.UpdateProgress(itemName, 1);
        
            // Update the description for the next step
            QuestManager.Instance.UpdateDescription(questName, nextStepDescription);

            if (collectSound != null) AudioSource.PlayClipAtPoint(collectSound, transform.position);
            UIManager.Instance.HideHoverText(); 
            Destroy(gameObject);
        }
    }
}