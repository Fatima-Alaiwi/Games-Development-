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
    public Quest loadTruckQuest;           // Drag your "LoadTruck" quest asset here!

    [Header("Voice Lines - Dialogue Clips")]
    public List<AudioClip> introLines = new List<AudioClip>();
    public List<AudioClip> investigateBuildingLines = new List<AudioClip>();
    public List<AudioClip> loadTruckLines = new List<AudioClip>();           // Lines tracking when player is loading the truck
    public List<AudioClip> restorePowerLines = new List<AudioClip>(); //
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
        if (_isSpeakingSequence) return;

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

        if (current.isCompleted)
        {
            StartCoroutine(PlayDialogueSequence(GetCompletionClips(current.questName), () =>
            {
                HandleQuestChainTransitions(current);
            }));
        }
        else
        {
            StartCoroutine(PlayDialogueSequence(GetTargetClips(current.questName), null));
        }
    }

    private void HandleQuestChainTransitions(Quest completedQuest)
    {
        if (QuestManager.Instance == null) return;

        switch (completedQuest.questName)
        {
            case "Investigate Building":
                // Hand in "Investigate Building"
                QuestManager.Instance.CompleteQuestPublic(completedQuest);
                
                // Immediately grant "LoadTruck" quest
                if (loadTruckQuest != null)
                {
                    QuestManager.Instance.AcceptQuest(loadTruckQuest);
                    Debug.Log("InfoBot: 'Investigate Building' handed in. 'LoadTruck' quest started!");
                }
                break;
        }
    }

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

                yield return new WaitForSeconds(currentClip.length);
            }
        }

        onSequenceComplete?.Invoke();
        _isSpeakingSequence = false;
    }

    private List<AudioClip> GetTargetClips(string questName)
    {
        switch (questName)
        {
            case "Investigate Building": return investigateBuildingLines;
            case "LoadTruck":            return loadTruckLines;        // Plays if player talks to bot while trying to load the truck
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
            case "Investigate Building": return restorePowerLines; 
            default:                     return new List<AudioClip>();
        }
    }

    private void PlayTalkAnimation()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Talk");
    }
}