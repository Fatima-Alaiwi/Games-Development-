using UnityEngine;

public class InfoBotNPC : MonoBehaviour, IInteractable
{
    [Header("Bot Settings")]
    public Transform _labelAnchor;
    public bool _isInteractable = true;

    [Header("Quest Assignment")]
    public Quest investigateBuildingQuest; // Drag the "Investigate Building" asset here

    public Quest restorePowerQuest;

    // Interface Requirements
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;
    public string InteractionText => "Communicate";

    public void Interact()
    {
        if (QuestManager.Instance.activeQuests.Count == 0)
        {
            GiveFirstQuest();
            return;
        }

        Quest current = QuestManager.Instance.activeQuests[0];

        // Check if the current quest is finished but still in the list
        if (current.isCompleted)
        {
            HandleQuestCompletion(current);
        }
        else
        {
            ExecuteConversation(current);
        }
}

    private void GiveFirstQuest()
    {
        Debug.Log("Robot: 'Welcome Voyager. The temporal rift is in the central tower. Go Investigate the Building.'");
        
        // This adds the quest to your manager's list
        QuestManager.Instance.AcceptQuest(investigateBuildingQuest);
        
        PlayTalkAnimation();
    }

    private void ExecuteConversation(Quest quest)
    {
        // Matching the names exactly to your uploaded images
        switch (quest.questName)
        {
            case "Investigate Building":
                Debug.Log("Player: 'The elevator is dead.'");
                Debug.Log("Robot: 'The power grid is offline. You must Restore Power.'");
                break;

            case "Restore Power":
                Debug.Log("Player: 'How do I fix the grid?'");
                Debug.Log("Robot: 'Go to the garage and use the car to Deliver the Cell.'");
                break;

            case "Repair Car":
                Debug.Log("Player: 'The car has a flat tire.'");
                Debug.Log("Robot: 'Find a replacement at the store so you can move the cell.'");
                break;

            case "Deliver Cell":
                Debug.Log("Player: 'Cell is loaded.'");
                Debug.Log("Robot: 'Security droids are active! You must Kill Robots to reach the lift.'");
                break;

            case "Kill Robots":
                Debug.Log("Player: 'The path is blocked!'");
                Debug.Log("Robot: 'Neutralize the threats and Enter the Portal.'");
                break;

            case "Enter Portal":
                Debug.Log("Robot: 'Safe travels, Voyager. The timeline depends on you.'");
                break;

            default:
                Debug.Log($"Robot: 'Processing data for {quest.questName}...'");
                break;
        }

        PlayTalkAnimation();
    }

    private void PlayTalkAnimation()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Talk");
    }

    private void HandleQuestCompletion(Quest completedQuest)
    {
        switch (completedQuest.questName)
        {
            case "Investigate Building":
                Debug.Log("Robot: 'The elevator is dead? As I feared. You must Restore Power.'");
            
                QuestManager.Instance.activeQuests.Remove(completedQuest);
            
                QuestManager.Instance.AcceptQuest(restorePowerQuest); 
                break;
            
        }
        PlayTalkAnimation();
    }
}