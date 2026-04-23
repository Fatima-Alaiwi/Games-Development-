using UnityEngine;

public class GoldenKeyPickup : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Pick up Golden Key";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Key Settings")]
    public Sprite keyIcon;
    public string keyItemName = "GoldenKey";
    public AudioClip collectSound;

    public void Interact()
    {
        // Add key to inventory
        bool added = InventoryManager.instance.AddItem(keyItemName, keyIcon);

        if (added)
        {
            // Update quest HUD counter
            if (QuestManager.Instance != null)
                QuestManager.Instance.UpdateQuestCount(keyItemName, 1);

            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Inventory full!");
        }
    }
}