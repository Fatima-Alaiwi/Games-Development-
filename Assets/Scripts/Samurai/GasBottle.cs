using UnityEngine;

public class GasBottle : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Pick up Gas Bottle";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Item Settings")]
    public string itemName = "Gas";
    public Sprite itemIcon;
    public AudioClip collectSound;

    [Header("Quest Settings")]
    public Quest gasQuest;

    public void Interact()
    {
        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            if (QuestManager.Instance != null && gasQuest != null)
                QuestManager.Instance.AcceptQuest(gasQuest);

            if (QuestManager.Instance != null)
                QuestManager.Instance.UpdateProgress(itemName, 1);

            gameObject.SetActive(false);
        }
    }
}