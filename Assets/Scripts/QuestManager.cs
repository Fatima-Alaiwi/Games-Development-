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

    // public void UpdateProgress(string goalName, int amount)
    // {
    //     foreach (Quest q in activeQuests)
    //     {
    //         if (q.goalItemName == goalName && !q.isCompleted)
    //         {
    //             q.currentAmount += amount;
    //             if (q.currentAmount >= q.goalAmount)
    //             {
    //                 CompleteQuest(q);
    //             }
    //         }
    //     }
    // }



//raghad
    public void UpdateProgress(string goalName, int amount)
{
    Quest questToComplete = null; // find first, complete after

    foreach (Quest q in activeQuests)
    {
        if (q.goalItemName == goalName && !q.isCompleted)
        {
            q.currentAmount += amount;
            if (q.currentAmount >= q.goalAmount)
                questToComplete = q; // don't complete inside the loop!
        }
    }

    if (questToComplete != null)
        CompleteQuest(questToComplete); // safe — loop is finished
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



    // public void CompleteQuest(Quest q)
    // {
    //     q.isCompleted = true;
    //     Debug.Log("Quest Finished: " + q.questName);
    //     // You can add gold/exp rewards here
    // }
    
//raghad
    public void CompleteQuest(Quest q)
{
    q.isCompleted = true;
    Debug.Log("Quest Finished: " + q.questName);

    if (!completedQuests.Contains(q))
        completedQuests.Add(q);

    if (activeQuests.Contains(q))
        activeQuests.Remove(q);
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

    public void UpdatedCompleteQuest(Quest q)
    {
        if (q == null) return;

        q.isCompleted = true;
        Debug.Log("Quest Finished: " + q.questName);

        // 1. Add to the completed list (so the Computer UI success marks work)
        if (!completedQuests.Contains(q))
        {
            completedQuests.Add(q);
        }

        // 2. REMOVE from the active list (this hides the HUD)
        if (activeQuests.Contains(q))
        {
            activeQuests.Remove(q);
        }
    }



//raghad
public bool IsQuestComplete(Quest quest)
{
    if (quest == null) return false;
    foreach (Quest q in activeQuests)
        if (q == quest && q.isCompleted) return true;
    foreach (Quest q in completedQuests)
        if (q == quest) return true;
    return false;
}
}