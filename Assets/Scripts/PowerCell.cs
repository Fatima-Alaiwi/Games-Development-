using UnityEngine;

public class PowerCell : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public string itemName = "PowerCell";
    public Sprite itemIcon;
    public AudioClip collectSound;

    [Header("Quest Data")]
    public Quest powerQuest;
    // Matching the KeycardItem's ability to update descriptions
    public string nextStepDescription = "Insert Power Cell into the Generator"; 

    [Header("Interaction Settings")]
    [field: SerializeField] public string InteractionText { get; set; } = "Pick up Power Cell";
    public bool isInteractable { get; set; } = true;
    
    [SerializeField] private Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.UpdateProgress(itemName, 1);
                
                QuestManager.Instance.UpdateQuestDescription(powerQuest.questName, nextStepDescription);
            }

            LockOtherCells();

            if (collectSound != null) 
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            UIManager.Instance.HideHoverText(); 
            
            Destroy(gameObject);
        }
    }

    private void LockOtherCells()
    {
        PowerCell[] allCells = FindObjectsOfType<PowerCell>();
        foreach (PowerCell cell in allCells)
        {
            if (cell != this)
            {
                cell.isInteractable = false;
                cell.InteractionText = "Already carrying a cell";
            }
        }
    }
}