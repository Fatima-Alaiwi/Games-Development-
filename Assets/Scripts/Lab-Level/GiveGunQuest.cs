using UnityEngine;

public class GiveGunQuest : VoiceLineTrigger
{
    [Header("Grant Quest Settings")]
    public Quest questToGive;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (playOnlyOnce && _hasPlayed) return;

            if (AreRequirementsMet())
            {
                GiveQuestAndSpeak();
            }
        }
    }

    private bool AreRequirementsMet()
    {
        if (requiredQuests == null || requiredQuests.Count == 0) return true;
        if (QuestManager.Instance == null) return false;

        foreach (Quest req in requiredQuests)
        {
            if (!QuestManager.Instance.IsQuestCompleted(req.questName))
            {
                return false; 
            }
        }
        return true;
    }

    private void GiveQuestAndSpeak()
    {
        if (QuestManager.Instance != null && questToGive != null)
        {
            QuestManager.Instance.AcceptQuest(questToGive);
            Debug.Log($"<color=green>QUEST GRANTED:</color> {questToGive.questName}");
        }

        base.PlayVoice();
    }
}