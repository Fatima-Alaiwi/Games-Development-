using UnityEngine;

public class InfoBotNPC : MonoBehaviour, IInteractable
{
    [Header("Components")]
    public AudioSource voiceSource;
    public Animator anim;

    [Header("Voice Clips")]
    public AudioClip firstMeetingClip;
    public AudioClip guidanceClip;

    [Header("Interface Settings")]
    [SerializeField] private string _interactionText = "Talk to Info Bot";
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;

    [Header("Animation Settings")]
    public int numberOfTalkAnimations = 3; // Set this to how many talk animations you have

    private bool hasMetPlayer = false;

    // --- IInteractable Implementation ---

    public string InteractionText => _interactionText;

    public bool isInteractable
    { 
        get => _isInteractable;
        set => _isInteractable = value;
    }

    public Transform LabelAnchor => _labelAnchor;

    public void Interact()
    {
        if (!hasMetPlayer)
        {
            StartGreeting();
        }
        else
        {
            ProvideGuidance();
        }
    }

    private void StartGreeting()
    {
        hasMetPlayer = true;
        _interactionText = "Ask for Guidance";

        if (anim) anim.SetTrigger("Wave");
        PlayVoice(firstMeetingClip);
    }

    private void ProvideGuidance()
    {
        if (anim) 
        {
            // Pick a random animation index for variety
            int randomIndex = Random.Range(0, numberOfTalkAnimations);
            anim.SetInteger("TalkIndex", randomIndex);
            anim.SetTrigger("Talk");
        }
        
        PlayVoice(guidanceClip);
    }

    private void PlayVoice(AudioClip clip)
    {
        if (clip != null && voiceSource != null)
        {
            voiceSource.clip = clip;
            voiceSource.Play();
        }
    }
}