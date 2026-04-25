using UnityEngine;

public class PickupObject : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to Pick Up";
    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Item Settings")]
    public string itemName;
    public Sprite itemIcon;
    public AudioClip collectSound;

    public void Interact()
    {
        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);
        if (added)
        {
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            if (QuestManager.Instance != null)
                QuestManager.Instance.UpdateProgress(itemName, 1);

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }
}