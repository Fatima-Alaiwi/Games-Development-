using UnityEngine;

public class PowerCell : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public string itemName = "PowerCell";
    public Sprite itemIcon;
    public AudioClip collectSound;

    [Header("Quest Data")]
    public Quest powerQuest;
    public string nextStepDescription = "Insert Power Cell into the Generator"; 

    [Header("Interaction Settings")]
    [field: SerializeField] public string InteractionText { get; set; } = "Locked in Housing";
    public bool isInteractable { get; set; } = false; 
    
    [SerializeField] private Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        if (!isInteractable) return;

        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            HandleQuestLogic();

            if (collectSound != null) 
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            UIManager.Instance.HideHoverText(); 
            
            // Lock all other cells so the player can only carry one
            LockRemainingCells();

            Destroy(gameObject);
        }
    }

    private void HandleQuestLogic()
    {
        if (QuestManager.Instance == null || powerQuest == null) return;

        if (!QuestManager.Instance.activeQuests.Contains(powerQuest))
        {
            QuestManager.Instance.activeQuests.Add(powerQuest);
        }

        QuestManager.Instance.UpdateQuestCount(itemName, 1);

        QuestManager.Instance.UpdateQuestDescription(powerQuest.questName, nextStepDescription);
    }

    private void LockRemainingCells()
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