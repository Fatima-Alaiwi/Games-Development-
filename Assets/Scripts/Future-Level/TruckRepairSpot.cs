using UnityEngine;

public class TruckRepairSpot : MonoBehaviour, IInteractable
{
    public GameObject visualWheel; // Drag the invisible wheel mesh from the truck here
    public string InteractionText => "Install Wheel";
    public Transform LabelAnchor => transform;
    public bool isInteractable { get; set; } = true;

    void Start()
    {
        // Ensure the wheel starts invisible
        if (visualWheel != null) visualWheel.SetActive(false);
    }

    public void Interact()
    {
        Quest active = QuestManager.Instance.activeQuests[0];

        // Check if player has the wheel (Check your inventory logic here)
        if (active.questName == "Repair Car" && active.currentAmount == 1)
        {
            // 1. Make the wheel visible on the truck
            visualWheel.SetActive(true);

            // 2. Complete the quest
            active.isCompleted = true;
            active.activeMessage = active.completeMessage; // "Return to Info-bot"

            // 3. Disable this repair spot so we can't click it again
            isInteractable = false;
            
            Debug.Log("Truck Repaired!");
        }
        else
        {
            Debug.Log("You need a wheel to fix this.");
        }
    }
}