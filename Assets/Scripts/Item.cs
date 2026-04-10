using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;

    public AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
{
    Debug.Log("Something entered trigger: " + other.name);

    if (other.CompareTag("Player"))
    {
        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Cannot collect item, inventory is full.");
        }
    }
}
}