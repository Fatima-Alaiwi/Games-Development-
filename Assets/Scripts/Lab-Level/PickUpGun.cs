using UnityEngine;

public class PickUpGun : MonoBehaviour, IInteractable
{
    [Header("Quest Settings")]
    public Quest gunQuest;

    [Header("IInteractable Settings")]
    [SerializeField] private string _interactionText = "Press E to Pick Up Gun";
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;

    [Header("Visuals")]
    public GameObject visualModel;

    // Interface Properties
    public string InteractionText => _interactionText;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;

    public void Interact()
    {
        // 1. Check if the QuestManager exists and has active quests
        if (QuestManager.Instance != null && QuestManager.Instance.activeQuests.Count > 0)
        {
            // 2. Only allow interaction if the gun quest is the FIRST active quest
            if (QuestManager.Instance.activeQuests[0] == gunQuest)
            {
                CompleteTheQuest();
            }
            else
            {
                Debug.Log("<color=yellow>INTERACTION BLOCKED:</color> This isn't your current objective.");
            }
        }
    }

    private void CompleteTheQuest()
    {
        // Complete the quest using your QuestManager method
        QuestManager.Instance.CompleteQuestPublic(gunQuest);
        
        Debug.Log($"<color=orange>INTERACTED:</color> {gunQuest.questName} is now complete.");

        // Hide the gun model
        if (visualModel != null)
            visualModel.SetActive(false);
        else
            gameObject.SetActive(false);

        // Turn off interactability so the hover text disappears
        isInteractable = false;
    }
}