using UnityEngine;

public class PickUpGun : MonoBehaviour, IInteractable
{
    [Header("Quest Settings")]
    public Quest gunQuest;

    [Header("IInteractable Settings")]
    [SerializeField] private string _interactionText = "Press E to Pick Up Gun";
    [SerializeField] private string _blockedInteractionText = "Not now";
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;

    [Header("Visuals")]
    public GameObject visualModel;

    // Interface Properties
    public string InteractionText => IsCurrentObjective() ? _interactionText : _blockedInteractionText;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;

    public void Interact()
    {
        if (IsCurrentObjective())
        {
            CompleteTheQuest();
        }
        else
        {
            Debug.Log("<color=yellow>INTERACTION BLOCKED:</color> This isn't your current objective.");
        }
    }

    private bool IsCurrentObjective()
    {
        return QuestManager.Instance != null &&
               QuestManager.Instance.activeQuests.Count > 0 &&
               QuestManager.Instance.activeQuests[0] == gunQuest;
    }

    private void CompleteTheQuest()
    {
        // 1. Safe backend completion through the manager
        QuestManager.Instance.CompleteQuestPublic(gunQuest);
        
        Debug.Log($"<color=orange>INTERACTED:</color> {gunQuest.questName} is now complete.");

        // 2. NEW: Tell the level-aware script to make the gun appear in the player's hands
        if (PlayerControllerGun.instance != null)
        {
            PlayerControllerGun.instance.UnlockAndEquipGun();
        }

        // 3. Hide the gun model from the floor
        if (visualModel != null)
            visualModel.SetActive(false);
        else
            gameObject.SetActive(false);

        isInteractable = false;
    }
}
