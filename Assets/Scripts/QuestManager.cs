using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<Quest> activeQuests = new List<Quest>();

    void Awake()
    {
        if (Instance == null) Instance = this;

        //raghad added this!!!!!!
        // Reset all quests when the game starts
        // foreach (Quest q in activeQuests)
        //     q.ResetQuest();
        
        // activeQuests.Clear();

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

    void CompleteQuest(Quest q)
    {
        q.isCompleted = true;
        Debug.Log("Quest Finished: " + q.questName);
        // You can add gold/exp rewards here
    }
}