using UnityEngine;
public class MaintenanceComputer : VoiceLineTrigger, IInteractable
{
    [Header("Quest Settings")]
    public string questGoalName = "ReadLogs";
    
    [Header("UI Panels")]
    public GameObject computerScreenUI; 

    [Header("Interaction Settings")]
    [field: SerializeField] public string InteractionText { get; set; } = "Access Maintenance Logs";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        if (computerScreenUI.activeSelf)
        {
            CloseComputer();
        }
        else
        {
            OpenComputer();
        }
    }

    void OpenComputer()
    {
        computerScreenUI.SetActive(true);
        InteractionText = "Press E to Exit Terminal";

        computerScreenUI.GetComponent<ComputerUIController>().RefreshLogs();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PlayerControllerGun.instance != null) 
        {
            PlayerControllerGun.instance.canMove = false;
        }

        if (playOnlyOnce)
        {
            if (!_hasPlayed)
            {
                base.PlayVoice();
            }
        }
        else
        {
            base.PlayVoice();
        }

        QuestManager.Instance.UpdateProgress(questGoalName, 1);
    }

    void CloseComputer()
    {
        computerScreenUI.SetActive(false);
        InteractionText = "Access Maintenance Logs";
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (PlayerControllerGun.instance != null) 
        {
            PlayerControllerGun.instance.canMove = true;
        }
    }
}