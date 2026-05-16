using UnityEngine;
using System.Collections.Generic;

public class InfoBotNPC : MonoBehaviour, IInteractable
{
    [Header("Bot Settings")]
    public Transform _labelAnchor;
    public bool _isInteractable = true;
    public AudioSource audioSource;

    [Header("Quest Assignment")]
    public Quest investigateBuildingQuest; 
    public Quest restorePowerQuest;

    [Header("Voice Lines - Dialogue Clips")]
    public List<AudioClip> introLines = new List<AudioClip>();
    public List<AudioClip> investigateBuildingLines = new List<AudioClip>();
    public List<AudioClip> restorePowerLines = new List<AudioClip>();
    public List<AudioClip> deliverCellLines = new List<AudioClip>();
    public List<AudioClip> killRobotsLines = new List<AudioClip>();
    public List<AudioClip> enterPortalLines = new List<AudioClip>();

    [Header("Voice Lines - Completion Transitions")]
    public List<AudioClip> investigateBuildingCompleteLines = new List<AudioClip>();

    // Sequential Tracking Variables
    private List<AudioClip> _currentConversationClips = new List<AudioClip>();
    private int _currentLineIndex = 0;
    private string _lastTrackedQuestName = "";
    private bool _isHandlingCompletion = false;

    // Interface Requirements
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;
    public string InteractionText => "Communicate";

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void Interact()
    {
        // 1. If audio is actively playing, block interaction so lines don't overlap
        if (audioSource != null && audioSource.isPlaying) return;

        // 2. Give the initial setup quest if no active objective exists
        if (QuestManager.Instance.activeQuests.Count == 0)
        {
            GiveFirstQuest();
            return;
        }

        Quest current = QuestManager.Instance.activeQuests[0];

        // 3. Handle quest hand-ins or standard progression
        if (current.isCompleted)
        {
            HandleQuestCompletion(current);
        }
        else
        {
            ExecuteConversation(current);
        }
    }

    private void GiveFirstQuest()
    {
        HandleSequencePlayback(introLines, "Intro", () => 
        {
            // Callback executed only when all intro voice lines finish playing sequentially
            if (QuestManager.Instance != null && investigateBuildingQuest != null)
            {
                QuestManager.Instance.AcceptQuest(investigateBuildingQuest);
            }
        });
    }

    private void ExecuteConversation(Quest quest)
    {
        _isHandlingCompletion = false;
        List<AudioClip> targetClips = GetTargetClips(quest.questName);
        HandleSequencePlayback(targetClips, quest.questName, null);
    }

    private void HandleQuestCompletion(Quest completedQuest)
    {
        _isHandlingCompletion = true;
        List<AudioClip> targetClips = GetCompletionClips(completedQuest.questName);

        HandleSequencePlayback(targetClips, completedQuest.questName + "_Complete", () =>
        {
            // Callback executed when completion dialogue finishes processing sequentially
            switch (completedQuest.questName)
            {
                case "Investigate Building":
                    if (QuestManager.Instance != null)
                    {
                        QuestManager.Instance.activeQuests.Remove(completedQuest);
                        if (restorePowerQuest != null)
                        {
                            QuestManager.Instance.AcceptQuest(restorePowerQuest);
                        }
                    }
                    break;
            }
        });
    }

    private void HandleSequencePlayback(List<AudioClip> clipList, string conversationKey, System.Action onSequenceComplete)
    {
        // Reset line counters if moving to a completely different dialogue conversation
        if (_lastTrackedQuestName != conversationKey)
        {
            _lastTrackedQuestName = conversationKey;
            _currentLineIndex = 0;
        }

        if (clipList == null || clipList.Count == 0)
        {
            // Execute the backend mechanics immediately if audio files are unassigned
            onSequenceComplete?.Invoke();
            return;
        }

        // Play the line corresponding to our index counter
        if (_currentLineIndex < clipList.Count)
        {
            AudioClip clipToPlay = clipList[_currentLineIndex];
            if (audioSource != null && clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay);
                PlayTalkAnimation();
            }

            _currentLineIndex++;

            // If that was the last line of the conversation block, execute transition states
            if (_currentLineIndex >= clipList.Count)
            {
                onSequenceComplete?.Invoke();
                _currentLineIndex = 0; // Loop conversation back to the beginning for subsequent interactions
            }
        }
    }

    private List<AudioClip> GetTargetClips(string questName)
    {
        switch (questName)
        {
            case "Investigate Building": return investigateBuildingLines;
            case "Restore Power":       return restorePowerLines;
            case "Deliver Cell":         return deliverCellLines;
            case "Kill Robots":          return killRobotsLines;
            case "Enter Portal":         return enterPortalLines;
            default:                     return new List<AudioClip>();
        }
    }

    private List<AudioClip> GetCompletionClips(string questName)
    {
        switch (questName)
        {
            case "Investigate Building": return investigateBuildingCompleteLines;
            default:                     return new List<AudioClip>();
        }
    }

    private void PlayTalkAnimation()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Talk");
    }
}