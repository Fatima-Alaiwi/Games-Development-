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

                if (QuestManager.Instance != null)
                    QuestManager.Instance.UpdateProgress(itemName, 1);

                GetComponent<Collectible>()?.MarkCollected();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Cannot collect item, inventory is full.");
            }
        }
    }

}