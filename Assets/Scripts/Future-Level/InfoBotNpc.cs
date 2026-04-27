using UnityEngine;

public class InfoBotNPC : MonoBehaviour, IInteractable
{
    [Header("Bot Settings")]
    public Transform _labelAnchor;
    public bool _isInteractable = true;

    // Interface Requirements
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;
    public string InteractionText => "Talk to Bot";

    public void Interact()
    {
        // 1. Get the first active quest object from your manager
        if (QuestManager.Instance.activeQuests.Count > 0)
        {
            Quest currentQuest = QuestManager.Instance.activeQuests[0];
            
            // 2. Pass the whole Quest object to the conversation logic
            ExecuteConversation(currentQuest);
        }
        else
        {
            Debug.Log("Robot: 'No active mission detected. Please check your log.'");
        }
    }

    private void ExecuteConversation(Quest quest)
    {
        // We use the Quest's name (or Title) to determine the dialogue
        // This keeps it abstract and lets you create "Small Quests" as states
        switch (quest.questName) 
        {
            case "FindEnergyCell": // Make sure this matches your Quest asset name exactly
                Debug.Log("Player: 'What is this place?'");
                Debug.Log("Robot: 'Analyzing... This is the sector core. You need the pulse key.'");
                break;

            case "RepairConsole":
                Debug.Log("Player: 'I found the component.'");
                Debug.Log("Robot: 'Excellent. Initiate the installation sequence at the console.'");
                break;

            case "ActivateGate":
                Debug.Log("Player: 'The gate is opening!'");
                Debug.Log("Robot: 'Move quickly, Voyager. The temporal window is closing.'");
                break;

            default:
                Debug.Log("Player: 'I have a task for you.'");
                Debug.Log($"Robot: 'Understood. Processing data for {quest.questName}.'");
                break;
        }

        // Trigger your Talk animation
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Talk");
    }
}