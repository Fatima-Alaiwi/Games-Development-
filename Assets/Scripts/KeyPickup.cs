using UnityEngine;
// Raghad: KeyPickup script — used on both key_1 and key_3
// In Inspector, set Inventory Item Name differently for each key:
//   key_1 → inventoryItemName = "HorrorKey_1"
//   key_3 → inventoryItemName = "HorrorKey_3"
public class KeyPickup : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Pick up Key";
    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Inventory Settings")]
    public Sprite keyIcon;
    public AudioClip collectSound;
    public string inventoryItemName = "HorrorKey_1"; // change to "HorrorKey_3" on key_3 in Inspector

    [Header("Quest Settings")]
    public string questGoalName = "key_1"; // change to "key_3" on key_3 in Inspector

    public void Interact()
    {
        // 1. Add to inventory with its specific name
        InventoryManager.instance.AddItem(inventoryItemName, keyIcon);

        // 2. Play sound
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // 3. Update quest
        QuestManager.Instance.UpdateProgress(questGoalName, 1);

        // 4. Hide the key
        gameObject.SetActive(false);

        Debug.Log("Key picked up: " + inventoryItemName);
    }
}