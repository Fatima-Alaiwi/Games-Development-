using UnityEngine;

public class BottlePickup : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to pick up bottle";

    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Quest Settings")]
    public Quest bottleQuest;

    [Header("Inventory Settings")]
    public Sprite bottleIcon;

    [Header("Sound")]
    public AudioClip collectSound;

    private Collectible _collectible;

    void Awake()
    {
        TryGetComponent(out _collectible);
    }

    public void Interact()
    {
        // 1. Add to inventory immediately — no quest check needed
        bool added = InventoryManager.instance.AddItem("Bottle", bottleIcon);

        if (!added)
        {
            Debug.Log("Inventory full!");
            return;
        }

        // 2. If the bottle quest is already active, update its progress
        if (bottleQuest != null && QuestManager.Instance.activeQuests.Contains(bottleQuest))
            QuestManager.Instance.UpdateProgress("Bottle", 1);

        // 3. Play collect sound
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // 4. Track as collected so save/load hides it, then hide it
        if (_collectible != null) _collectible.MarkCollected();
        gameObject.SetActive(false);

        Debug.Log("Bottle picked up!");
    }
}