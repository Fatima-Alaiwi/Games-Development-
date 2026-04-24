using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();



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

        public void CompleteQuestPublic(Quest q)
    {
        if (!activeQuests.Contains(q)) return;
        CompleteQuest(q);
    }


    // Updates HUD counter only, does NOT complete the quest
    public void UpdateQuestCount(string goalName, int amount)
    {
        foreach (Quest q in activeQuests)
        {
            if (q == null) continue;
            if (q.goalItemName == goalName && !q.isCompleted)
            {
                q.currentAmount += amount;
                // Clamp so it never auto-completes
                q.currentAmount = Mathf.Min(q.currentAmount, q.goalAmount);
                Debug.Log($"Quest count updated: {q.questName} {q.currentAmount}/{q.goalAmount}");
            }
        }
    }

    public bool IsQuestCompleted(string name)
{
    // Search the completed list specifically
    return completedQuests.Exists(x => x.questName == name);
}
    public void CompleteQuest(Quest q)
    {
        q.isCompleted = true;
        Debug.Log("Quest Finished: " + q.questName);
        // You can add gold/exp rewards here
    }

    public void UpdateQuestDescription(string questName, string newDescription)
    {
        foreach (Quest q in activeQuests)
        {
            if (q != null && q.questName == questName)
            {
                // The HUD reads activeMessage every frame, 
                // so changing it here updates the screen instantly.
                q.activeMessage = newDescription; 
                return;
            }
        }
    }
}