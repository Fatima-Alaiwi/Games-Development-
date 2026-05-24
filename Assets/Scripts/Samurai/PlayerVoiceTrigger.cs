using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerVoiceTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip voiceLine;

    [Header("Settings")]
    public bool playOnlyOnce = true;
    public string playerTag = "Player";
    public float delay = 0f; // set this in Inspector per trigger

    [Header("Conditions — leave empty to ignore")]
    public List<Quest> requiredActiveQuests = new List<Quest>();
    public List<Quest> requiredCompletedQuests = new List<Quest>();

    private bool _hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (playOnlyOnce && _hasPlayed) return;
        if (!CanPlay()) return;

        StartCoroutine(PlayWithDelay());
    }

    IEnumerator PlayWithDelay()
    {
        yield return new WaitForSeconds(delay);
        Play();
    }

    bool CanPlay()
    {
        if (requiredActiveQuests.Count > 0)
        {
            if (QuestManager.Instance == null) return false;
            bool anyActive = false;
            foreach (var quest in requiredActiveQuests)
            {
                if (QuestManager.Instance.activeQuests.Contains(quest))
                {
                    anyActive = true;
                    break;
                }
            }
            if (!anyActive) return false;
        }

        if (requiredCompletedQuests.Count > 0)
        {
            if (QuestManager.Instance == null) return false;
            foreach (var quest in requiredCompletedQuests)
            {
                if (!QuestManager.Instance.completedQuests.Contains(quest))
                    return false;
            }
        }

        return true;
    }

    void Play()
    {
        if (audioSource == null || voiceLine == null)
        {
            Debug.LogWarning($"PlayerVoiceTrigger on {gameObject.name}: missing AudioSource or AudioClip!");
            return;
        }

        audioSource.PlayOneShot(voiceLine);
        _hasPlayed = true;
        Debug.Log($"<color=cyan>VOICE TRIGGERED:</color> {voiceLine.name}");
    }
}