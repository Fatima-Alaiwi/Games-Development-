using UnityEngine;

public class BottlePickup : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to pick up bottle";

    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Quest Settings")]
    public Quest bottleQuest; // Drag BottleQuest here

    [Header("Inventory Settings")]
    public Sprite bottleIcon; // Drag the bottle icon/sprite here

    public Quest requiredQuest; // drag "Kill All Enemies" quest here in Inspector

    public void Interact()
    {
        if (!QuestManager.Instance.IsQuestComplete(requiredQuest))
    {
        UIManager.Instance.ShowHoverText("I should look around first...", transform.position);
        return;
    }
    
        // 1. Accept quest if not already active
        if (bottleQuest != null && !QuestManager.Instance.activeQuests.Contains(bottleQuest))
            QuestManager.Instance.AcceptQuest(bottleQuest);

        // 2. Update quest progress
        if (bottleQuest != null)
            QuestManager.Instance.UpdateProgress("Bottle", 1);

        // 3. Add to inventory (needs name + icon)
        InventoryManager.instance.AddItem("Bottle", bottleIcon);

        // 4. Hide the bottle
        gameObject.SetActive(false);

        Debug.Log("Bottle picked up!");
    }
}