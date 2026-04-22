using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "QuestSystem/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea] public string description;
    public string goalItemName; // What do they need to find/interact with?
    public string activeMessage; //The message that appears in the quest panel while the quest requirements are not met
    public string completeMessage; //The message that appears in the quest panel if the quest requirements are met
    public int goalAmount;      // How many?
    public int currentAmount;   // How many do they have now?
    public bool isCompleted;

    public void ResetQuest()
    {
        currentAmount = 0;
        isCompleted = false;
    }
}