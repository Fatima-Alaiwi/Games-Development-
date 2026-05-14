using UnityEngine;

public class VoiceLineTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip voiceLine;
    
    [Header("Quest Requirements")]
    [Tooltip("If empty, plays regardless. If assigned, only plays if this quest is FIRST in the active list.")]
    public Quest requiredQuest; 

    [Header("Configuration")]
    public string playerTag = "Player";
    public bool playOnlyOnce = true;

    private bool _hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (playOnlyOnce && _hasPlayed) return;

            if (CanPlayVoice())
            {
                PlayVoice();
            }
        }
    }

    private bool CanPlayVoice()
    {
        // If no quest is dragged, play regardless
        if (requiredQuest == null) return true;

        // Check if QuestManager exists and has active quests
        if (QuestManager.Instance != null && QuestManager.Instance.activeQuests.Count > 0)
        {
            // Specifically check if the FIRST quest in the list matches our dragged quest
            return QuestManager.Instance.activeQuests[0] == requiredQuest;
        }

        return false;
    }

    private void PlayVoice()
    {
        if (audioSource != null && voiceLine != null)
        {
            audioSource.PlayOneShot(voiceLine);
            _hasPlayed = true;
            Debug.Log($"<color=cyan>VOICE:</color> Playing {voiceLine.name}");
        }
        else
        {
            Debug.LogWarning("VOICE TRIGGER: Missing AudioSource or AudioClip component!");
        }
    }
}