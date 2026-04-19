using UnityEngine;

public class QuestItem : MonoBehaviour, IInteractable
{
    public string itemName;
    
    [field: SerializeField] 
    public string InteractionText { get; set; } = "Pick up Item";
    
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        // Tell the manager we found one!
        QuestManager.Instance.UpdateProgress(itemName, 1);
        
        // Hide the item or destroy it
        gameObject.SetActive(false); 
    }
}