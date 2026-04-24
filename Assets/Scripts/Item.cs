using UnityEngine;
public class Item : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool added = InventoryManager.instance.AddItem(itemName, itemIcon);
            if (added)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
                
                // ✅ Also update quest progress if there's an active quest for this item
                if (QuestManager.Instance != null)
                    QuestManager.Instance.UpdateProgress(itemName, 1);

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Cannot collect item, inventory is full.");
            }
        }
    }

}