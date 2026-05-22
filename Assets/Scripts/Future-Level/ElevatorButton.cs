using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;
    [SerializeField] private string _defaultInteractionText = "Check Elevator";
    [SerializeField] private string _callElevatorText = "Press [E] to Call Elevator";
    [SerializeField] private string _waitingForPowerText = "Power Offline";

    [Header("Quest Requirement")]
    public string investigateBuildingQuestName = "Investigate Building";
    public string killRobotsQuestName = "KillRobots";

    [Header("Elevator Doors")]
    public Transform leftDoorPart;
    public Transform rightDoorPart;
    public float doorSlideDistance = 1.5f;
    public float doorOpenSpeed = 2f;
    public Vector3 leftDoorOpenDirection = Vector3.back;
    public Vector3 rightDoorOpenDirection = Vector3.forward;
    public GameObject elevatorInteriorInteractable;

    [Header("Audio Setup")]
    public AudioSource playerAudioSource; // Drag the Player's AudioSource component here
    public List<AudioClip> elevatorInvestigationLines = new List<AudioClip>();

    private bool _isSpeaking = false;
    private bool _isOpening = false;
    private bool _hasOpened = false;
    private Vector3 _leftDoorClosedWorldPosition;
    private Vector3 _rightDoorClosedWorldPosition;
    private Vector3 _leftDoorOpenWorldPosition;
    private Vector3 _rightDoorOpenWorldPosition;

    // Interface Requirements
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;
    public string InteractionText => GetInteractionText();

    private void Start()
    {
        if (leftDoorPart != null)
        {
            _leftDoorClosedWorldPosition = leftDoorPart.position;
            _leftDoorOpenWorldPosition = _leftDoorClosedWorldPosition + leftDoorOpenDirection.normalized * doorSlideDistance;
        }

        if (rightDoorPart != null)
        {
            _rightDoorClosedWorldPosition = rightDoorPart.position;
            _rightDoorOpenWorldPosition = _rightDoorClosedWorldPosition + rightDoorOpenDirection.normalized * doorSlideDistance;
        }

        if (elevatorInteriorInteractable != null)
        {
            elevatorInteriorInteractable.SetActive(true);
            SetElevatorInteriorInteractable(false);
        }
    }

    public void Interact()
    {
        if (_isSpeaking || _isOpening || _hasOpened) return;

        if (CanCallElevator())
        {
            StartCoroutine(OpenElevatorDoors());
            return;
        }

        if (QuestManager.Instance != null && QuestManager.Instance.activeQuests.Count > 0)
        {
            Quest active = QuestManager.Instance.activeQuests[0];

            if (active != null && active.questName == investigateBuildingQuestName)
            {
                active.isCompleted = true;
                active.currentAmount = 1;
                active.activeMessage = active.completeMessage;

                Debug.Log("Elevator: Power Offline. HUD Updated to: " + active.activeMessage);

                StartCoroutine(PlayVoiceLinesSequence());
            }
        }
    }

    private string GetInteractionText()
    {
        if (_hasOpened) return "Elevator Ready";
        if (CanCallElevator()) return _callElevatorText;

        if (QuestManager.Instance != null && QuestManager.Instance.activeQuests.Count > 0)
        {
            Quest active = QuestManager.Instance.activeQuests[0];
            if (active != null && active.questName == investigateBuildingQuestName)
            {
                return _defaultInteractionText;
            }
        }

        return _waitingForPowerText;
    }

    private bool IsQuestCompleted(string questName)
    {
        if (QuestManager.Instance == null || string.IsNullOrEmpty(questName)) return false;
        return QuestManager.Instance.IsQuestCompleted(questName);
    }

    private bool CanCallElevator()
    {
        return IsQuestCompleted(killRobotsQuestName);
    }

    private IEnumerator OpenElevatorDoors()
    {
        _isOpening = true;

        while (!DoorsReachedOpenPosition())
        {
            if (leftDoorPart != null)
            {
                leftDoorPart.position = Vector3.MoveTowards(
                    leftDoorPart.position,
                    _leftDoorOpenWorldPosition,
                    doorOpenSpeed * Time.deltaTime);
            }

            if (rightDoorPart != null)
            {
                rightDoorPart.position = Vector3.MoveTowards(
                    rightDoorPart.position,
                    _rightDoorOpenWorldPosition,
                    doorOpenSpeed * Time.deltaTime);
            }

            yield return null;
        }

        if (elevatorInteriorInteractable != null)
        {
            elevatorInteriorInteractable.SetActive(true);
            SetElevatorInteriorInteractable(true);
        }

        _hasOpened = true;
        _isInteractable = false;
        _isOpening = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideHoverText();
        }
    }

    private bool DoorsReachedOpenPosition()
    {
        bool leftDone = leftDoorPart == null || Vector3.Distance(leftDoorPart.position, _leftDoorOpenWorldPosition) < 0.01f;
        bool rightDone = rightDoorPart == null || Vector3.Distance(rightDoorPart.position, _rightDoorOpenWorldPosition) < 0.01f;
        return leftDone && rightDone;
    }

    private void SetElevatorInteriorInteractable(bool state)
    {
        if (elevatorInteriorInteractable == null) return;

        IInteractable interactable = elevatorInteriorInteractable.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.isInteractable = state;
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

                yield return new WaitForSeconds(currentClip.length);
            }
        }

        _isSpeaking = false;
    }
}
