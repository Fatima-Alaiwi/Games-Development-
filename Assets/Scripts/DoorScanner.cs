using UnityEngine;

public class DoorScanner : MonoBehaviour, IInteractable
{
    [Header("Quest Assignment")]
    public Quest portalQuest; 
    
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
        QuestManager.Instance.AcceptQuest(portalQuest);

        QuestManager.Instance.UpdateProgress(portalQuest.goalItemName, 1);
        QuestManager.Instance.CompleteQuest(portalQuest);
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

        QuestManager.Instance.UpdateProgress(portalQuest.goalItemName, 1);

        InventoryManager.instance.RemoveItem(portalQuest.goalItemName);

        isInteractable = false;
}

    void DeniedSequence()
    {
        if (audioSource != null) audioSource.PlayOneShot(accessDeniedSound);

        if (QuestManager.Instance != null && portalQuest != null)
        {
            QuestManager.Instance.AcceptQuest(portalQuest);
        }
    }
}