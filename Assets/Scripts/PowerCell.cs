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
    [field: SerializeField] public string InteractionText { get; set; } = "Pick up Power Cell";
    public bool isInteractable { get; set; } = true;
    
    [SerializeField] private Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            HandleQuestLogic();

            if (collectSound != null) 
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            UIManager.Instance.HideHoverText(); 
            LockOtherCells();
            Destroy(gameObject);
        }
    }

    private void HandleQuestLogic()
    {
        if (QuestManager.Instance == null || powerQuest == null) return;

        bool questActive = QuestManager.Instance.activeQuests.Contains(powerQuest);

        if (!questActive)
        {
            QuestManager.Instance.activeQuests.Add(powerQuest);
            powerQuest.isCompleted = false;
            powerQuest.currentAmount = 0;
        }

        QuestManager.Instance.UpdateQuestCount(itemName, 1);

        QuestManager.Instance.UpdateQuestDescription(powerQuest.questName, nextStepDescription);
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