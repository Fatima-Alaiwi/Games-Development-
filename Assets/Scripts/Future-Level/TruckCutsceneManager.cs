using System.Collections;
using UnityEngine;

public class TruckCutsceneManager : MonoBehaviour
{
    public Camera cutsceneCamera;
    public Transform drone;
    public GameObject powerCellOnTruck;
    public GameObject powerCellOnDrone;

    public string powerCellItemName = "PowerCell";
    public string completionStepDescription = "Quest Complete: Power cell stolen by drone.";

    [Header("Post-Cutscene Quest Trigger")]
    public ZoneTrigger killRobotsZoneTrigger;

    [Header("Post-Cutscene Voice Line")]
    public AudioSource voiceLineAudioSource;
    public AudioClip postCutsceneVoiceLine;

    public Transform playerCutsceneStandingPoint;
    public Transform pointA; 
    public Transform pointB; 
    public Transform pointC; 

    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    private bool _isCutscenePlaying = false;
    private MonoBehaviour _savedPlayerScript;
    private Camera _savedPlayerCamera;
    private Quest _questToComplete;

    void Start()
    {
        if (powerCellOnDrone != null)
        {
            powerCellOnDrone.SetActive(false);
        }
    }

    public void SetupPlayerReferences(MonoBehaviour playerScript, Camera playerCamera)
    {
        _savedPlayerScript = playerScript;
        _savedPlayerCamera = playerCamera;
    }

    public void SetQuestToComplete(Quest associatedQuest)
    {
        _questToComplete = associatedQuest;
    }

    public void StartCutscene()
    {
        if (_isCutscenePlaying) return;
        StartCoroutine(PlayCutsceneSequence());
    }

    private IEnumerator PlayCutsceneSequence()
    {
        _isCutscenePlaying = true;

        if (_savedPlayerScript != null && playerCutsceneStandingPoint != null)
        {
            CharacterController charController = _savedPlayerScript.GetComponent<CharacterController>();
            if (charController != null) charController.enabled = false;

            _savedPlayerScript.transform.position = playerCutsceneStandingPoint.position;
            _savedPlayerScript.transform.rotation = playerCutsceneStandingPoint.rotation;

            if (charController != null) charController.enabled = true;

            SetPlayerCanMove(false);
        }

        if (cutsceneCamera != null)
        {
            if (_savedPlayerCamera != null) _savedPlayerCamera.gameObject.SetActive(false);
            cutsceneCamera.gameObject.SetActive(true);
        }

        if (drone != null && pointA != null)
        {
            drone.position = pointA.position;
            drone.rotation = pointA.rotation;
        }

        yield return new WaitForSeconds(0.5f); 

        if (pointB != null)
        {
            yield return StartCoroutine(MoveAndRotateDrone(pointB.position, pointB.rotation));
        }

        if (powerCellOnTruck != null) powerCellOnTruck.SetActive(false);
        if (powerCellOnDrone != null) powerCellOnDrone.SetActive(true);
        
        yield return new WaitForSeconds(0.6f); 

        if (pointC != null && drone != null)
        {
            Quaternion targetTurnRotation = Quaternion.LookRotation(pointC.position - drone.position);
            yield return StartCoroutine(RotateDroneOnly(targetTurnRotation));
        }

        if (pointC != null)
        {
            yield return StartCoroutine(MoveAndRotateDrone(pointC.position, pointC.rotation));
        }

        EndCutscene();
    }

    private IEnumerator MoveAndRotateDrone(Vector3 targetPosition, Quaternion targetRotation)
    {
        while (Vector3.Distance(drone.position, targetPosition) > 0.05f)
        {
            drone.position = Vector3.MoveTowards(drone.position, targetPosition, moveSpeed * Time.deltaTime);
            drone.rotation = Quaternion.RotateTowards(drone.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        drone.position = targetPosition;
    }

    private IEnumerator RotateDroneOnly(Quaternion targetRotation)
    {
        while (Quaternion.Angle(drone.rotation, targetRotation) > 0.1f)
        {
            drone.rotation = Quaternion.RotateTowards(drone.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        drone.rotation = targetRotation;
    }

    private void EndCutscene()
    {
        _isCutscenePlaying = false;
        
        if (cutsceneCamera != null && cutsceneCamera.gameObject.activeSelf)
        {
            cutsceneCamera.gameObject.SetActive(false);
            if (_savedPlayerCamera != null)
            {
                _savedPlayerCamera.gameObject.SetActive(true);
            }
        }

        if (drone != null)
        {
            drone.gameObject.SetActive(false);
        }

        SetPlayerCanMove(true);

        if (QuestManager.Instance != null && _questToComplete != null)
        {
            QuestManager.Instance.UpdateQuestDescription(_questToComplete.questName, completionStepDescription);

            _questToComplete.currentAmount = _questToComplete.goalAmount;

            QuestManager.Instance.CompleteQuestPublic(_questToComplete);
        }
        else if (QuestManager.Instance != null)
        {
            QuestManager.Instance.UpdateQuestCount(powerCellItemName, 1);
        }

        EnableKillRobotsZoneTrigger();
        PlayPostCutsceneVoiceLine();
    }

    private void SetPlayerCanMove(bool canMove)
    {
        if (_savedPlayerScript == null) return;

        var field = _savedPlayerScript.GetType().GetField("canMove");
        if (field != null)
        {
            field.SetValue(_savedPlayerScript, canMove);
        }
    }

    private void EnableKillRobotsZoneTrigger()
    {
        if (killRobotsZoneTrigger == null) return;

        killRobotsZoneTrigger.gameObject.SetActive(true);
        killRobotsZoneTrigger.enabled = true;
    }

    private void PlayPostCutsceneVoiceLine()
    {
        if (voiceLineAudioSource != null && postCutsceneVoiceLine != null)
        {
            voiceLineAudioSource.PlayOneShot(postCutsceneVoiceLine);
        }
    }
}
