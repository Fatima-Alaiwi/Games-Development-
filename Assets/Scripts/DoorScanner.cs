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
    bool hasKeycard = InventoryManager.instance.HasItem(portalQuest.goalItemName);

    if (hasKeycard)
    {
        // 1. If they have the card but never "started" the quest, start it now
        QuestManager.Instance.AcceptQuest(portalQuest);

        // 2. Immediately complete it
        QuestManager.Instance.UpdateProgress(portalQuest.goalItemName, 1);

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

        // 1. Mark progress (This triggers CompleteQuest in the manager)
        QuestManager.Instance.UpdateProgress(portalQuest.goalItemName, 1);

        // 2. Remove the card from the inventory
        InventoryManager.instance.RemoveItem(portalQuest.goalItemName);

        // 3. Hide the Quest Panel (Call your UI script here)
        // QuestUI.Instance.HidePanel(); 
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