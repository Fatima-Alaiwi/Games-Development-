using UnityEngine;
using System.Collections;

public class FairyNPC : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] private string _interactionText = "Press E to talk to the Fairy";
    public string InteractionText => _interactionText;
    public bool isInteractable { get; set; } = true;

    [SerializeField] private Transform _labelAnchor;
    public Transform LabelAnchor => _labelAnchor != null ? _labelAnchor : transform;

    [Header("Audio & Voice Lines")]
    [Tooltip("Peter's reaction voice line after the Fairy speaks")]
    public AudioClip peterReactionClip;
    [Tooltip("The voice line file (.mp3, .wav) to play when interacting")]
    public AudioClip voiceLineClip;
    private AudioSource audioSource;

    [Header("Quest Progression Chain")]
    [Tooltip("The quest the player currently has to find her (e.g., Quest_TalkToFairy)")]
    public Quest talkToFairyQuest;
    [Tooltip("The new quest she hands over (e.g., Quest_TalkToMerchant)")]
    public Quest talkToMerchantQuest;

    [Header("Reward")]
    [Tooltip("The Health Pickup Prefab to spawn when interaction finishes")]
    public GameObject healthPickupPrefab;
    [Tooltip("Where the health pickup drops from")]
    public Transform spawnPoint;

    [Header("Wing Transforms & Settings")]
    public Transform leftWing;
    public Transform rightWing;
    [Tooltip("Flap rotation angle for the LEFT wing")]
    public float leftFlapAngle = 25f;
    [Tooltip("Flap rotation angle for the RIGHT wing")]
    public float rightFlapAngle = 25f;

    public float flapSpeedIdle = 6f;
    public float flapSpeedTalking = 20f;

    [Header("Arm Transforms & Settings")]
    public Transform leftArm;
    public Transform rightArm;
    [Tooltip("Drop rotation angle for the LEFT arm")]
    public float leftArmDropAngle = 55f;
    [Tooltip("Drop rotation angle for the RIGHT arm")]
    public float rightArmDropAngle = 55f;

    [Header("Hover Bobbing Settings")]
    public float bobAmount = 0.2f;
    public float bobSpeed = 3f;

    private float currentFlapSpeed;
    private float wingProgress = 0f;
    private Vector3 initialLocalPosition;
    private bool isTalking = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D sound optimization for FPS perspective
    }

    void Start()
    {
        currentFlapSpeed = flapSpeedIdle;
        initialLocalPosition = transform.localPosition;

        if (_labelAnchor == null) _labelAnchor = transform;
    }

    void Update()
    {
        // Calculate dynamic wing speed depending on dialogue state
        float targetSpeed = isTalking ? flapSpeedTalking : flapSpeedIdle;
        currentFlapSpeed = Mathf.Lerp(currentFlapSpeed, targetSpeed, Time.deltaTime * 4f);
        wingProgress += Time.deltaTime * currentFlapSpeed;

        // --- Separate Left and Right Wing Flapping ---
        float leftWingSin = Mathf.Sin(wingProgress) * leftFlapAngle;
        float rightWingSin = Mathf.Sin(wingProgress) * rightFlapAngle;

        if (leftWing != null) leftWing.localRotation = Quaternion.Euler(180f, leftWingSin, 0);
        if (rightWing != null) rightWing.localRotation = Quaternion.Euler(0, rightWingSin, 0);

        // --- Separate Left and Right Arm Angle Drops ---
        if (leftArm != null) leftArm.localRotation = Quaternion.Euler(0, 0, -leftArmDropAngle);
        if (rightArm != null) rightArm.localRotation = Quaternion.Euler(0, 0, rightArmDropAngle);

        // Hover Bobbing
        float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.localPosition = initialLocalPosition + new Vector3(0, bobOffset, 0);
    }

    public void Interact()
    {
        if (!isInteractable) return;
        StartCoroutine(TalkRoutine());
    }

    private IEnumerator TalkRoutine()
    {
        isInteractable = false;
        isTalking = true;

        Debug.Log("Peter interacted with the Fairy!");

        // 1. Complete the 'Talk To Fairy' Quest if it exists
        if (talkToFairyQuest != null)
        {
            talkToFairyQuest.isCompleted = true;

            // Move the quest to the completed list so Raghad's QuestHUD displays 'completeMessage'
            if (!QuestManager.Instance.completedQuests.Contains(talkToFairyQuest))
            {
                QuestManager.Instance.completedQuests.Add(talkToFairyQuest);
            }

            // Clean it out of the active list so it stops rendering the "0/1" progress numbers
            if (QuestManager.Instance.activeQuests.Contains(talkToFairyQuest))
            {
                QuestManager.Instance.activeQuests.Remove(talkToFairyQuest);
            }
        }

        // 2. Play her custom voice line audio track
        float dialogueDuration = 2.5f;
        if (audioSource != null && voiceLineClip != null)
        {
            audioSource.PlayOneShot(voiceLineClip);
            dialogueDuration = voiceLineClip.length;
        }

        // Wait exactly for the duration of her speaking line
        yield return new WaitForSeconds(dialogueDuration);

        // 3. Dialogue has finished: Stop fast wing-flapping animations instantly
        isTalking = false;
        // Play Peter's reaction line right after fairy finishes
        if (peterReactionClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(peterReactionClip);
            yield return new WaitForSeconds(peterReactionClip.length);
        }

        // 4. Drop the health reward pack right on cue when speech stops!
        if (healthPickupPrefab != null)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position - Vector3.up * 0.5f;
            Instantiate(healthPickupPrefab, spawnPos, Quaternion.identity);
            Debug.Log("Potion spawned successfully!");
        }

        // 5. Instantly hand over the 'Talk to Merchant' Quest to update HUD layout
        if (talkToMerchantQuest != null && QuestManager.Instance != null)
        {
            talkToMerchantQuest.isCompleted = false;
            QuestManager.Instance.AcceptQuest(talkToMerchantQuest);
            Debug.Log($"New Quest Active: {talkToMerchantQuest.questName}");
        }

        _interactionText = "Fairy: Go see the Merchant in town, Peter!";

        // Brief cooldown pause before allowing player interaction input resets
        yield return new WaitForSeconds(2f);
        isInteractable = true;
    }
}