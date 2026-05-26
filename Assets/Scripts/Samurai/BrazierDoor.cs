using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrazierDoor : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string _interactionText = "The door is sealed...";
    public string InteractionText => _interactionText;
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Spawner")]
    public EnemySpawner brazierSpawner;

    [Header("Requirements")]
    public BrazierGate brazierGate;
    public string requiredKeyName = "Key1";
    public List<Quest> requiredQuests = new List<Quest>();

    [Header("Door Settings")]
    public float openAngle = -90f;
    public float openSpeed = 2f;

    [Header("Sound")]
    public AudioClip openingDoorClip;

    [Header("Voice Line")]
    public AudioClip voiceLine;

    private AudioSource audioSource;
    private bool isOpen = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (isOpen) return;

        bool braziersLit = brazierGate != null && brazierGate.BothLit();
        bool allQuestsDone = AllQuestsDone();
        bool hasKey = InventoryManager.instance != null &&
                      InventoryManager.instance.HasItem(requiredKeyName);

        if (!braziersLit)
            _interactionText = "The braziers must be lit first...";
        else if (!allQuestsDone)
            _interactionText = "Complete all quests first...";
        else if (!hasKey)
            _interactionText = "You need a key to open this gate.";
        else
            _interactionText = "Unlock the gate [E]";
    }

    public void Interact()
    {
        if (isOpen) return;

        bool braziersLit = brazierGate != null && brazierGate.BothLit();
        bool allQuestsDone = AllQuestsDone();
        bool hasKey = InventoryManager.instance != null &&
                      InventoryManager.instance.HasItem(requiredKeyName);

        if (!braziersLit || !allQuestsDone || !hasKey) return;

        InventoryManager.instance.RemoveItem(requiredKeyName, 1);

        if (brazierGate != null)
            brazierGate.PlayOpenSound();

        OpenDoor();
    }

    bool AllQuestsDone()
    {
        if (requiredQuests.Count == 0) return false;
        if (QuestManager.Instance == null) return false;
        foreach (var quest in requiredQuests)
        {
            if (quest == null) continue;
            if (!QuestManager.Instance.IsQuestCompleted(quest.questName))
                return false;
        }
        return true;
    }

    void OpenDoor()
    {
        isOpen = true;
        isInteractable = false;
        _interactionText = "The gate is open!";

        if (openingDoorClip != null && audioSource != null)
            audioSource.PlayOneShot(openingDoorClip);

        if (voiceLine != null)
        {
            GameObject voiceObj = GameObject.Find("VoiceAudioSource");
            if (voiceObj != null)
            {
                AudioSource voiceSource = voiceObj.GetComponent<AudioSource>();
                if (voiceSource != null)
                    voiceSource.PlayOneShot(voiceLine);
            }
        }

        if (brazierSpawner != null)
            brazierSpawner.StartSpawning();

        StartCoroutine(OpenDoorCoroutine());
    }

    IEnumerator OpenDoorCoroutine()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, openAngle, 0);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }
        transform.rotation = endRotation;
    }
}
