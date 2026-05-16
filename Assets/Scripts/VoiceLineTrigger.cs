using UnityEngine;
using System.Collections.Generic;

public class VoiceLineTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip voiceLine;
    
    [Header("Quest Requirements")]
    [Tooltip("If the list is empty, plays regardless. If quests are added, plays only if the CURRENT active quest (Index 0) is in this list.")]
    public List<Quest> requiredQuests = new List<Quest>(); 

    [Header("Configuration")]
    public string playerTag = "Player";
    public bool playOnlyOnce = true;

    protected bool _hasPlayed = false;

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
        if (requiredQuests == null || requiredQuests.Count == 0) return true;

        if (QuestManager.Instance != null && QuestManager.Instance.activeQuests.Count > 0)
        {
            Quest currentActive = QuestManager.Instance.activeQuests[0];

            return requiredQuests.Contains(currentActive);
        }

        return false;
    }

    protected virtual void PlayVoice()
    {
        if (audioSource != null && voiceLine != null)
        {
            audioSource.PlayOneShot(voiceLine);
            _hasPlayed = true;
            Debug.Log($"<color=cyan>VOICE TRIGGERED:</color> {voiceLine.name}");
        }
        else
        {
            Debug.LogWarning("VOICE TRIGGER: Missing AudioSource or AudioClip!");
        }
    }
}