using UnityEngine;

public class PowerControlPanel : MonoBehaviour, IInteractable
{
    [Header("Quest Assignment")]
    public Quest powerQuest;
    
    [Header("Target References")]
    public PowerCore targetCore;

    [Header("Interaction Settings")]
    [field: SerializeField] public string InteractionText { get; set; } = "Initialize Power Sequence";
    public bool isInteractable { get; set; } = true;
    
    [SerializeField] private Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        // 1. Start the Quest via the Manager
        if (QuestManager.Instance != null && powerQuest != null)
        {
            QuestManager.Instance.AcceptQuest(powerQuest);
            Debug.Log("Quest Started: " + powerQuest.questName);
        }
        
        // 2. Find EVERY PowerCell in the map and unlock them globally
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
        InteractionText = "Sequence Active";
        
        UIManager.Instance.HideHoverText();
    }
}