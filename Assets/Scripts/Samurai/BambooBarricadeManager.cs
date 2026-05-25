using UnityEngine;

public class BambooBarricadeManager : MonoBehaviour
{
    [Header("Bamboos to cut")]
    public BarricadeBamboo[] bamboos; // drag all 5 golden bamboos here

    [Header("Barricade Collider")]
    public Collider barricadeCollider; // drag BambooBarricade box collider object here

    [Header("Portal")]
    public SamuraiPortalController portal;

    [Header("Quest")]
    public Quest bambooQuest;

    private int cutCount = 0;

    void Start()
    {
        // Give each bamboo a reference to this manager
        foreach (var b in bamboos)
            if (b != null) b.SetManager(this);
    }

    public void OnBambooCut()
    {
        cutCount++;
        Debug.Log($"Bamboos cut: {cutCount}/{bamboos.Length}");

        // Update quest progress
        if (QuestManager.Instance != null && bambooQuest != null)
        {
            if (!QuestManager.Instance.activeQuests.Contains(bambooQuest) &&
                !QuestManager.Instance.completedQuests.Contains(bambooQuest))
                QuestManager.Instance.AcceptQuest(bambooQuest);

            QuestManager.Instance.UpdateProgress("Bamboo", 1);
        }

        if (cutCount >= bamboos.Length)
            ClearBarricade();
    }

    void ClearBarricade()
    {
        // Disable the physical wall
        if (barricadeCollider != null)
            barricadeCollider.enabled = false;

        // Complete quest
        if (QuestManager.Instance != null && bambooQuest != null)
            QuestManager.Instance.CompleteQuestPublic(bambooQuest);

        // Activate portal
        if (portal != null)
            portal.ActivatePortal();

        Debug.Log("Barricade cleared! Portal activated.");
    }
}