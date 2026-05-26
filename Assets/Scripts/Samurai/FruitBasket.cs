using UnityEngine;

public class FruitBasket : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Pick up Fruit Basket";

    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Item Settings")]
    public string itemName = "Fruit";
    public Sprite itemIcon;
    public AudioClip collectSound;

    [Header("Quest Settings")]
    public Quest fruitQuest;

    public void Interact()
    {
        Debug.Log("FruitBasket Interact() called!");

        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);
        Debug.Log("Item added: " + added);

        if (added)
        {
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            if (QuestManager.Instance != null && fruitQuest != null && QuestManager.Instance.activeQuests.Count == 0)
            {
                QuestManager.Instance.AcceptQuest(fruitQuest);
            }

            if (QuestManager.Instance != null)
                QuestManager.Instance.UpdateProgress(itemName, 1);

            GetComponent<Collectible>()?.MarkCollected();
            gameObject.SetActive(false);
        }
    }
}