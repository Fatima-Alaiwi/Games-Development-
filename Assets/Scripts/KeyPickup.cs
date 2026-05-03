using UnityEngine;
//raghad 
public class KeyPickup : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Pick up Key";
    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Inventory Settings")]
    public Sprite keyIcon; // drag key icon here in Inspector
    public AudioClip collectSound; // drag collect sound here

    [Header("Quest Settings")]
    public string questGoalName = "key_1"; // change to "key_3" on the library key in Inspector

    public void Interact()
    {
        // 1. Add to inventory
        InventoryManager.instance.AddItem("HorrorKey", keyIcon);

        // 2. Play sound
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // 3. Update quest — uses whatever questGoalName is set in Inspector
        QuestManager.Instance.UpdateProgress(questGoalName, 1);

        // 4. Hide the key
        gameObject.SetActive(false);

        Debug.Log("Key picked up: " + questGoalName);
    }
}