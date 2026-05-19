using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;

    [Header("Audio Setup")]
    public AudioSource playerAudioSource; // Drag the Player's AudioSource component here
    public List<AudioClip> elevatorInvestigationLines = new List<AudioClip>();

    private bool _isSpeaking = false;

    // Interface Requirements
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;
    public string InteractionText => "Check Elevator";

    public void Interact()
    {
        // Block overlapping audio interaction if the lines are already playing
        if (_isSpeaking) return;

        if (QuestManager.Instance.activeQuests.Count > 0)
        {
            Quest active = QuestManager.Instance.activeQuests[0];

            if (active.questName == "Investigate Building")
            {
                active.isCompleted = true;
                active.currentAmount = 1;
                active.activeMessage = active.completeMessage;

                Debug.Log("Elevator: Power Offline. HUD Updated to: " + active.activeMessage);

                // Automatically kick off the player's voice lines sequence
                StartCoroutine(PlayVoiceLinesSequence());
            }
            else if (active.questName == "Enter Portal")
            {
                Debug.Log("Elevator: Power Restored. Moving...");
            }
        }
    }

    private IEnumerator PlayVoiceLinesSequence()
    {
        if (elevatorInvestigationLines == null || elevatorInvestigationLines.Count == 0) yield break;

        _isSpeaking = true;

        for (int i = 0; i < elevatorInvestigationLines.Count; i++)
        {
            AudioClip currentClip = elevatorInvestigationLines[i];

            if (currentClip != null && playerAudioSource != null)
            {
                playerAudioSource.clip = currentClip;
                playerAudioSource.Play();

                // Wait until the line finishes completely before moving to the next one
                yield return new WaitForSeconds(currentClip.length);
            }
        }

        _isSpeaking = false;
    }
}