using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfoBotNPC : MonoBehaviour, IInteractable
{
    [Header("Bot Settings")]
    public Transform _labelAnchor;
    public bool _isInteractable = true;
    public AudioSource audioSource;
    public Animator animator;

    [Header("Animation")]
    public string idleStateName = "infot-bot-idle";
    public string talkStateName = "info-bot-talk-1";
    public string waveStateName = "info-bot-wave";
    public float waveDuration = 1.2f;
    public float animationFadeTime = 0.15f;

    [Header("Quest Assignment")]
    public Quest investigateBuildingQuest;
    public Quest loadTruckQuest;      

    [Header("Voice Lines - Dialogue Clips")]
    public List<AudioClip> introLines = new List<AudioClip>();
    public List<AudioClip> investigateBuildingLines = new List<AudioClip>();
    public List<AudioClip> loadTruckLines = new List<AudioClip>();         
    public List<AudioClip> restorePowerLines = new List<AudioClip>(); 
    public List<AudioClip> deliverCellLines = new List<AudioClip>();
    public List<AudioClip> killRobotsLines = new List<AudioClip>();
    public List<AudioClip> enterPortalLines = new List<AudioClip>();

    [Header("Voice Lines - Completion Transitions")]
    public List<AudioClip> investigateBuildingCompleteLines = new List<AudioClip>();

    private bool _isSpeakingSequence = false;
    private bool _hasPlayedGreetingWave = false;

    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;
    public string InteractionText => "Press [E] To Talk";

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        PlayIdleAnimation();
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
            }, true));
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
                QuestManager.Instance.CompleteQuestPublic(completedQuest);
                
                if (loadTruckQuest != null)
                {
                    QuestManager.Instance.AcceptQuest(loadTruckQuest);
                    Debug.Log("InfoBot: 'Investigate Building' handed in. 'LoadTruck' quest started!");
                }
                break;
        }
    }

    private IEnumerator PlayDialogueSequence(List<AudioClip> clipList, System.Action onSequenceComplete, bool playGreetingWave = false)
    {
        if (clipList == null || clipList.Count == 0)
        {
            onSequenceComplete?.Invoke();
            yield break;
        }

        _isSpeakingSequence = true;
        bool shouldWaveFirst = playGreetingWave && !_hasPlayedGreetingWave;
        if (!shouldWaveFirst)
        {
            PlayTalkAnimation();
        }

        for (int i = 0; i < clipList.Count; i++)
        {
            AudioClip currentClip = clipList[i];

            if (currentClip != null && audioSource != null)
            {
                audioSource.clip = currentClip;
                audioSource.Play();

                if (shouldWaveFirst && i == 0)
                {
                    PlayWaveAnimation();
                    _hasPlayedGreetingWave = true;

                    float waveTime = Mathf.Min(waveDuration, currentClip.length);
                    yield return new WaitForSeconds(waveTime);

                    PlayTalkAnimation();

                    float remainingClipTime = currentClip.length - waveTime;
                    if (remainingClipTime > 0f)
                    {
                        yield return new WaitForSeconds(remainingClipTime);
                    }
                }
                else
                {
                    PlayTalkAnimation();
                    yield return new WaitForSeconds(currentClip.length);
                }
            }
        }

        onSequenceComplete?.Invoke();
        _isSpeakingSequence = false;
        PlayIdleAnimation();
    }

    private List<AudioClip> GetTargetClips(string questName)
    {
        switch (questName)
        {
            case "Investigate Building": return investigateBuildingLines;
            case "LoadTruck":            return loadTruckLines;        
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
        PlayAnimationState(talkStateName);
    }

    private void PlayWaveAnimation()
    {
        PlayAnimationState(waveStateName);
    }

    private void PlayIdleAnimation()
    {
        PlayAnimationState(idleStateName);
    }

    private void PlayAnimationState(string stateName)
    {
        if (animator == null || string.IsNullOrEmpty(stateName)) return;

        animator.CrossFadeInFixedTime(stateName, animationFadeTime);
    }
}
