using UnityEngine;

public class DoorScanner : MonoBehaviour, IInteractable
{
    [Header("Quest Assignment")]
    public Quest portalQuest; // Drag your 'PortalAccessQuest' asset here in the Inspector
    
    [Header("Interactivity")]
    [field: SerializeField] public string InteractionText { get; set; } = "Scan Keycard";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Door & Screen")]
    public SlidingDoor doorScript;
    public GameObject screenCube;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip accessDeniedSound;
    public AudioClip accessGrantedSound;

    public void Interact()
    {
        // Check inventory for the keycard
        bool hasKeycard = InventoryManager.instance.HasItem(portalQuest.goalItemName);

        if (hasKeycard)
        {
            OpenDoorSequence();
        }
        else
        {
            DeniedSequence();
        }
    }

    void OpenDoorSequence()
    {
        if (audioSource != null) audioSource.PlayOneShot(accessGrantedSound);
        if (doorScript != null) doorScript.Open();
        if (screenCube != null) screenCube.SetActive(false);

        // Update progress in the manager
        QuestManager.Instance.UpdateProgress(portalQuest.goalItemName, 1);
        isInteractable = false;
    }

    void DeniedSequence()
    {
        if (audioSource != null) audioSource.PlayOneShot(accessDeniedSound);

        // Start the quest safely using the asset
        if (QuestManager.Instance != null && portalQuest != null)
        {
            QuestManager.Instance.AcceptQuest(portalQuest);
        }
    }
}