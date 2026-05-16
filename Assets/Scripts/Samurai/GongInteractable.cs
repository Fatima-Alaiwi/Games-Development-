using UnityEngine;

public class GongInteractable : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "The gong stand is damaged...";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Requirements")]
    public Quest bambooQuest;
    public string requiredItemName = "Bamboo";
    public int requiredAmount = 5;

    [Header("Portal")]
    public SamuraiPortalController portal;

    [Header("Audio")]
    public AudioClip repairSound;
    public AudioClip gongStrikeSound;

    private AudioSource audioSource;
    private bool isRepaired = false;
    private bool hasBeenStruck = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (hasBeenStruck)
        {
            InteractionText = "";
            isInteractable = false;
            return;
        }

        if (!isRepaired)
        {
            int count = GetBambooCount();
            InteractionText = count >= requiredAmount
                ? "Repair the Gong Stand [E]"
                : $"Need bamboo poles to repair ({count}/{requiredAmount})";
        }
        else
        {
            InteractionText = "Strike the Gong [E]";
        }
    }

    public void Interact()
    {
        if (hasBeenStruck) return;

        if (!isRepaired)
        {
            TryRepair();
            return;
        }

        StrikeGong();
    }

    void TryRepair()
    {
        if (GetBambooCount() < requiredAmount) return;

        InventoryManager.instance.RemoveItem(requiredItemName, requiredAmount);

        if (QuestManager.Instance != null && bambooQuest != null)
            QuestManager.Instance.CompleteQuestPublic(bambooQuest);

        if (repairSound != null)
            audioSource.PlayOneShot(repairSound);

        isRepaired = true;
    }

    void StrikeGong()
    {
        hasBeenStruck = true;
        isInteractable = false;

        if (gongStrikeSound != null)
            audioSource.PlayOneShot(gongStrikeSound);

        if (portal != null)
            portal.ActivatePortal();
    }

    int GetBambooCount()
    {
        if (InventoryManager.instance == null) return 0;
        foreach (var item in InventoryManager.instance.items)
            if (item.itemName == requiredItemName)
                return item.count;
        return 0;
    }
}