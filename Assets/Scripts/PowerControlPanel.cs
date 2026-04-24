using UnityEngine;

public class PowerControlPanel : MonoBehaviour, IInteractable
{
    public Quest powerQuest;
    public PowerCore targetCore;

    [field: SerializeField] public string InteractionText { get; set; } = "Initialize Power Sequence";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        QuestManager.Instance.AcceptQuest(powerQuest);
        
        if(targetCore != null) targetCore.ActivateCoreInteraction();

        isInteractable = false; // Panel is done
        InteractionText = "Sequence Initialized";
    }
}