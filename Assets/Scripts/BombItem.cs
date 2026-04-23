using UnityEngine;

public class BombItem : MonoBehaviour
{
    public string itemName = "DungeonBomb";
    public Sprite itemIcon;
    public AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            // Update dragon quest bomb counter
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