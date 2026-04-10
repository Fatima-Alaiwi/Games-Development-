using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered trigger: " + other.name);
        
        if (other.CompareTag("Player"))
        {
            InventoryManager.instance.AddItem(itemName, itemIcon);
            Destroy(gameObject); // disappear after pickup
        }
    }
}