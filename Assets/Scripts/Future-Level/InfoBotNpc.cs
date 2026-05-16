using UnityEngine;
using System.Collections;
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

    private bool _isSpeakingSequence = false;

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
        // 1. Block interaction if a sequence is already automatically running
        if (_isSpeakingSequence) return;

        // 2. Give the initial setup quest if no active objective exists
        if (QuestManager.Instance.activeQuests.Count == 0)
        {
            StartCoroutine(PlayDialogueSequence(introLines, () => 
            {
                if (QuestManager.Instance != null && investigateBuildingQuest != null)
                {
                    QuestManager.Instance.AcceptQuest(investigateBuildingQuest);
                }
            }));
            return;
        }

        Quest current = QuestManager.Instance.activeQuests[0];

        // 3. Handle quest hand-ins or standard progression
        if (current.isCompleted)
        {
            StartCoroutine(PlayDialogueSequence(GetCompletionClips(current.questName), () =>
            {
                switch (current.questName)
                {
                    case "Investigate Building":
                        if (QuestManager.Instance != null)
                        {
                            QuestManager.Instance.activeQuests.Remove(current);
                            if (restorePowerQuest != null)
                            {
                                QuestManager.Instance.AcceptQuest(restorePowerQuest);
                            }
                        }
                        break;
                }
            }));
        }
        else
        {
            StartCoroutine(PlayDialogueSequence(GetTargetClips(current.questName), null));
        }
    }

    // This Coroutine handles the automatic "once-and-done" playback loop
    private IEnumerator PlayDialogueSequence(List<AudioClip> clipList, System.Action onSequenceComplete)
    {
        if (clipList == null || clipList.Count == 0)
        {
            onSequenceComplete?.Invoke();
            yield break;
        }

        _isSpeakingSequence = true;

        for (int i = 0; i < clipList.Count; i++)
        {
            AudioClip currentClip = clipList[i];

            if (currentClip != null && audioSource != null)
            {
                audioSource.clip = currentClip;
                audioSource.Play();
                PlayTalkAnimation();

                // Wait right here until this specific clip finishes playing before moving to the next loop iteration
                yield return new WaitForSeconds(currentClip.length);
            }
        }

        // Run quest backend modifications after the entire audio track sequence is done
        onSequenceComplete?.Invoke();
        _isSpeakingSequence = false;
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