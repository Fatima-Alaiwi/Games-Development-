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

    [Header("Spawner")]
    public EnemySpawner brazierSpawner;
    public int requiredGasCount = 2; // how many gas bottles needed before spawning

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

            GetComponent<Collectible>()?.MarkCollected();

            // Check how many gas bottles collected
            int gasCount = GetGasCount();
            if (brazierSpawner != null && gasCount >= requiredGasCount)
                brazierSpawner.StartSpawning();

            gameObject.SetActive(false);
        }
    }

    int GetGasCount()
    {
        if (InventoryManager.instance == null) return 0;
        foreach (var item in InventoryManager.instance.items)
            if (item.itemName == itemName)
                return item.count;
        return 0;
    }
}