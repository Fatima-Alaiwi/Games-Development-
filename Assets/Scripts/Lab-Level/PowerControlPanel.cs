using UnityEngine;

public class PowerControlPanel : MonoBehaviour, IInteractable
{
    [Header("Quest Requirements")]
    public Quest requiredPortalRoomQuest;

    [Header("Quest Assignment")]
    public Quest powerQuest;
    
    [Header("Target References")]
    public PowerCore targetCore;

    [Header("Interaction Settings")]
    [SerializeField] private string _defaultInteractionText = "Initialize Power Sequence";
    private bool _sequenceActive = false;
    public bool isInteractable { get; set; } = true;
    
    [SerializeField] private Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public string InteractionText
    {
        get
        {
            if (_sequenceActive) return "Sequence Active";
            
            // If the prerequisite quest isn't done yet, display "Come back later"
            if (requiredPortalRoomQuest != null && QuestManager.Instance != null)
            {
                if (!QuestManager.Instance.IsQuestCompleted(requiredPortalRoomQuest.questName))
                {
                    return "Come back later";
                }
            }

            return _defaultInteractionText;
        }
        set => _defaultInteractionText = value;
    }

    public void Interact()
    {
        if (!isInteractable) return;

        // Double check requirement condition inside interaction execution
        if (requiredPortalRoomQuest != null && QuestManager.Instance != null)
        {
            if (!QuestManager.Instance.IsQuestCompleted(requiredPortalRoomQuest.questName))
            {
                Debug.Log("<color=yellow>PANEL LOCKED:</color> Prerequisite objective incomplete.");
                return; 
            }
        }

        if (QuestManager.Instance != null && powerQuest != null)
        {
            QuestManager.Instance.AcceptQuest(powerQuest);
            Debug.Log("Quest Started: " + powerQuest.questName);
        }
        
        PowerCell[] allCells = FindObjectsOfType<PowerCell>();
        
        if (allCells.Length > 0)
        {
            foreach (PowerCell cell in allCells)
            {
                cell.isInteractable = true;
                cell.InteractionText = "Press [E] to Pick up Power Cell";
            }
        }
        else
        {
            Debug.LogWarning("No PowerCells found in the scene to unlock!");
        }

        if (targetCore != null) 
        {
            targetCore.ActivateCoreInteraction();
        }

        isInteractable = false; 
        _sequenceActive = true;
        
        UIManager.Instance.HideHoverText();
    }
}