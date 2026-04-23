using UnityEngine;

public class GoldIngot : MonoBehaviour
{
    public string itemName = "DungeonGold";
    public Sprite itemIcon;
    public AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered trigger: " + other.name);
        if (!other.CompareTag("Player")) return;

        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            // Update quest HUD counter without completing the quest
            if (QuestManager.Instance != null)
                QuestManager.Instance.UpdateQuestCount(itemName, 1);

            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Cannot collect item, inventory is full.");
        }
    }
}