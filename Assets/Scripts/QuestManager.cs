using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<Quest> activeQuests = new List<Quest>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AcceptQuest(Quest quest)
    {
        if (!activeQuests.Contains(quest))
        {
            quest.ResetQuest();
            activeQuests.Add(quest);
            Debug.Log("Quest Started: " + quest.questName);
        }
    }

    public void UpdateProgress(string goalName, int amount)
    {
        Debug.Log("Updating progress for: " + goalName);
        Debug.Log("Active Quests Count: " + activeQuests.Count);
        foreach (Quest q in activeQuests)
        {
            if (q == null) continue; // Safety check
            if (q.goalItemName == goalName && !q.isCompleted)
            {
                q.currentAmount += amount;
                if (q.currentAmount >= q.goalAmount)
                {
                    CompleteQuest(q);
                }
            }
        }
    }

    void CompleteQuest(Quest q)
    {
    q.isCompleted = true;
    Debug.Log("Quest Finished: " + q.questName);

    // After a 2-second delay, remove the quest from the list
    // so the UI stops showing it.
    StartCoroutine(RemoveQuestAfterDelay(q, 2f));
    }

    private System.Collections.IEnumerator RemoveQuestAfterDelay(Quest q, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeQuests.Remove(q);
        // Trigger your UI refresh here!
    }

    public void UpdateDescription(string questName, string newDesc)
    {
        Quest q = activeQuests.Find(x => x != null && x.questName == questName);
        if (q != null)
        {
            q.description = newDesc;
            Debug.Log($"Quest '{questName}' description updated to: {newDesc}");
        }
    }
}