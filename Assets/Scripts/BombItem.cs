using UnityEngine;

public class BombItem : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Pick up Bomb";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public string itemName = "DungeonBomb";
    public Sprite itemIcon;
    public AudioClip collectSound;

    public void Interact()
    {
        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            if (QuestManager.Instance != null)
                QuestManager.Instance.UpdateQuestCount(itemName, 1);

            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Cannot collect item, inventory is full.");
        }
    }
}