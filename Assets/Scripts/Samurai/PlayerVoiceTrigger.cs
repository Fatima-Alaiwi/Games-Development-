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
    public float delay = 0f;

    [Header("Conditions — leave empty to ignore")]
    public List<Quest> requiredActiveQuests = new List<Quest>();
    public List<Quest> requiredCompletedQuests = new List<Quest>();

    private bool _hasPlayed = false;
    private bool _playerIsInside = false;

    void Start()
    {
        // Continuously poll conditions in background
        // This handles the case where the quest completes
        // while the player is already inside the trigger
        StartCoroutine(PollConditions());
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerIsInside = true;
        TryPlay();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerIsInside = false;
    }

    // Polls every 0.5s instead of every frame — cheap and reliable
    IEnumerator PollConditions()
    {
        while (!_hasPlayed)
        {
            yield return new WaitForSeconds(0.5f);

            if (_playerIsInside)
                TryPlay();
        }
    }

    void TryPlay()
    {
        if (_hasPlayed) return;
        if (!CanPlay()) return;
        StartCoroutine(PlayWithDelay());
    }

    IEnumerator PlayWithDelay()
    {
        if (_hasPlayed) yield break;
        _hasPlayed = true; // block duplicates immediately

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
            Debug.LogWarning($"PlayerVoiceTrigger on {gameObject.name}: " +
                             $"missing AudioSource or AudioClip!");
            return;
        }

        audioSource.PlayOneShot(voiceLine);
        Debug.Log($"<color=cyan>VOICE TRIGGERED:</color> {voiceLine.name}");
    }
}