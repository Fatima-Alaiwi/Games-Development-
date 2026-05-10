using UnityEngine;

public class WheelPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Pick up Wheel";
    [SerializeField] private Transform _labelAnchor;
    [SerializeField] private bool _isInteractable = true;

    public string InteractionText => _interactionText;
    public Transform LabelAnchor => _labelAnchor;
    public bool isInteractable 
    { 
        get => _isInteractable; 
        set => _isInteractable = value; 
    }

    public void Interact()
    {
        Quest active = QuestManager.Instance.activeQuests[0];
        if (active != null && active.questName == "Repair Car")
        {
            active.currentAmount = 1;
            active.activeMessage = "Install the wheel on the truck";
        }

        Debug.Log("Wheel picked up!");
        gameObject.SetActive(false); 
    }
}