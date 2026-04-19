using UnityEngine;

public class QuestGiver : MonoBehaviour, IInteractable
{
    public Quest questToGive;
    
    [field: SerializeField] 
    public string InteractionText { get; set; } = "Talk to NPC";
    
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        if (questToGive.isCompleted)
        {
            InteractionText = "Thank you for your help!";
            return;
        }

        QuestManager.Instance.AcceptQuest(questToGive);
        InteractionText = "Check back once you're done!";
    }
}