using UnityEngine;

public class SatellitePanel : MonoBehaviour, IInteractable
{
    [Header("Quest Requirements")]
    public Quest requiredPortalRoomQuest; // Drag the "Open the Portal Room" quest asset here

    public SatelliteController satelliteScript;

    [SerializeField] private string _defaultInteractionText = "Align Satellite";
    private bool _isOffline = false;
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public string InteractionText
    {
        get
        {
            if (_isOffline) return "System Offline - Alignment Complete";

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

        if (requiredPortalRoomQuest != null && QuestManager.Instance != null)
        {
            if (!QuestManager.Instance.IsQuestCompleted(requiredPortalRoomQuest.questName))
            {
                Debug.Log("<color=yellow>SATELLITE LOCKED:</color> Prerequisite objective incomplete.");
                return; 
            }
        }

        if (satelliteScript != null)
        {
            satelliteScript.StartControlling();
        }
    }

    public void SetPanelOffline()
    {
        isInteractable = false;
        _isOffline = true;
    }
}