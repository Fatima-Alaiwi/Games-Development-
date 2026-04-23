using UnityEngine;

public class MaintenanceComputer : MonoBehaviour, IInteractable
{
    [Header("Quest Settings")]
    public string questGoalName = "ReadLogs";
    
    [Header("UI Panels")]
    public GameObject computerScreenUI; // The UI with the maintenance messages

    [Header("Interaction Settings")]
    [field: SerializeField] public string InteractionText { get; set; } = "Access Maintenance Logs";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        // 1. Open the Computer UI
        if (computerScreenUI != null)
        {
            computerScreenUI.SetActive(true);
            
            // Unlock mouse so they can read/click exit
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // If you have a PlayerMovement reference, freeze the player
            PlayerControllerGun.instance.canMove = false; 
        }

        // 2. Complete the investigation quest
        // This tells the QuestManager that "ReadLogs" is done.
        QuestManager.Instance.UpdateProgress(questGoalName, 1);
        
        Debug.Log("Quest Progress: Maintenance logs read.");
    }
}