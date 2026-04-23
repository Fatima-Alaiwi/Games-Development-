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
        // You can add gold/exp rewards here
    }

    public void UpdateDescription(string questName, string newDesc)
{
    Quest q = activeQuests.Find(x => x.questName == questName);
    if (q != null) q.description = newDesc;
}
}