using UnityEngine;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;

    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;
    public string InteractionText => "Check Elevator";

    public void Interact()
    {
        if (QuestManager.Instance.activeQuests.Count > 0)
        {
            Quest active = QuestManager.Instance.activeQuests[0];

            if (active.questName == "Investigate Building")
            {
                // 1. Mark as completed for the Manager
                active.isCompleted = true;
                active.currentAmount = 1;

                // 2. Direct Access: Overwrite the message so the HUD script picks it up
                // This ensures 'active.activeMessage' now says "Return to Info-bot"
                active.activeMessage = active.completeMessage;

                Debug.Log("Elevator: Power Offline. HUD Updated to: " + active.activeMessage);
            }
            else if (active.questName == "Enter Portal")
            {
                Debug.Log("Elevator: Power Restored. Moving...");
                // Elevator movement logic here
            }
        }
    }
}