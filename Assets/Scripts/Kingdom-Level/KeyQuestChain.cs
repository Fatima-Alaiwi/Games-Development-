using UnityEngine;

public class KeyQuestChain : MonoBehaviour
{
    [Header("Quest Chain")]
    public Quest questToComplete;  // Drag "Find Key" quest here
    public Quest questToStart;     // Drag "Find Map" quest here

    void OnDestroy()
    {
        // PickupObject calls Destroy(gameObject) on successful pickup
        // so this fires at exactly the right moment
        if (QuestManager.Instance == null) return;

        if (questToComplete != null)
            QuestManager.Instance.CompleteQuest(questToComplete);

        if (questToStart != null)
            QuestManager.Instance.AcceptQuest(questToStart);
    }
}