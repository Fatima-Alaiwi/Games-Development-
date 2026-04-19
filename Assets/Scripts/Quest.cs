using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "QuestSystem/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea] public string description;
    public string goalItemName; // What do they need to find/interact with?
    public int goalAmount;      // How many?
    public int currentAmount;   // How many do they have now?
    
    public bool isCompleted;

    public void ResetQuest()
    {
        currentAmount = 0;
        isCompleted = false;
    }
}