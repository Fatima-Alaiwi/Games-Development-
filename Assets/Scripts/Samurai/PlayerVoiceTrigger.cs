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
    [Tooltip("Frames to wait before activating this trigger. Set to 2 for enter-level voices so the save system moves the player before the trigger listens.")]
    public int startDelayFrames = 0;
    public string playerTag = "Player";
    public float delay = 0f;

    [Header("Conditions — leave empty to ignore")]
    public List<Quest> requiredActiveQuests = new List<Quest>();
    public List<Quest> requiredCompletedQuests = new List<Quest>();

    private bool _hasPlayed = false;
    private bool _playerIsInside = false;
    private bool _isReady = false;

    void Start()
    {
        if (startDelayFrames > 0)
            StartCoroutine(DelayedStart());
        else
        {
            _isReady = true;
            StartCoroutine(PollConditions());
        }
    }

    IEnumerator DelayedStart()
    {
        for (int i = 0; i < startDelayFrames; i++)
            yield return null;

        _isReady = true;

        // Player may already be inside the trigger — check manually
        var box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Vector3 worldCenter = transform.TransformPoint(box.center);
            Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, transform.lossyScale);
            foreach (var hit in Physics.OverlapBox(worldCenter, halfExtents, transform.rotation))
            {
                if (hit.CompareTag(playerTag))
                {
                    _playerIsInside = true;
                    break;
                }
            }
        }

        StartCoroutine(PollConditions());
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_isReady) return;
        if (!other.CompareTag(playerTag)) return;
        _playerIsInside = true;
        TryPlay();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerIsInside = false;
    }

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
        _hasPlayed = true;

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
        Debug.Log($"<color=cyan>VOICE TRIGGERED:</color> {voiceLine.name}");
    }
}
