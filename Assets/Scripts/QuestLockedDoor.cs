using UnityEngine;

public class QuestLockedDoor : MonoBehaviour, IInteractable
{
    [Header("Quest Settings")]
    public Quest doorQuest;          // Drag the Key_1 quest asset here
    public string requiredKeyName = "Key_1";

    [Header("Interaction")]
    [field: SerializeField]
    public string InteractionText { get; set; } = "The door is locked. Find the key.";
    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Door Animation (optional)")]
    public Animator doorAnimator;            // Optional: assign if you have an open animation
    public string openAnimationTrigger = "Open";

    private bool isOpen = false;

    public void Interact()
    {
        if (isOpen) return;

        // Step 1: Give the quest when player first interacts with the door
        if (!activeQuests_ContainsDoorQuest())
        {
            QuestManager.Instance.AcceptQuest(doorQuest);
            InteractionText = $"Find the {requiredKeyName} to open this door.";
            return;
        }

        // Step 2: Check if the quest is completed (key was collected)
        if (doorQuest.isCompleted)
        {
            OpenDoor();
        }
        else
        {
            InteractionText = $"You still need the {requiredKeyName}. ({doorQuest.currentAmount}/{doorQuest.goalAmount})";
        }
    }

    private bool activeQuests_ContainsDoorQuest()
    {
        return QuestManager.Instance != null &&
               QuestManager.Instance.activeQuests.Contains(doorQuest);
    }

    private void OpenDoor()
    {
        isOpen = true;
        isInteractable = false;
        InteractionText = "";

        if (doorAnimator != null)
            doorAnimator.SetTrigger(openAnimationTrigger);
        else
            gameObject.SetActive(false); // Fallback: just hide the door

        Debug.Log("Door unlocked and opened!");
    }
}