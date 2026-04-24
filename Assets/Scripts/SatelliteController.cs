using UnityEngine;

public class SatelliteController : MonoBehaviour
{
    [Header("Quest Settings")]
    public Quest satelliteQuest;
    public Camera satelliteViewCamera; 
    public Camera mainPlayerCamera;    
    public SatellitePanel controlPanel;

    [Header("Movement Parts")]
    public Transform satelliteBase;    
    public Transform satelliteHead;

    [Header("Target Angles")]
    public float targetHorizontal = 45f;
    public float targetVertical = 32f;
    public float tolerance = 2f;
    private bool isControlling = false;
    private float currentH = 0f;
    private float currentV = 0f;

    void Start()
    {
        if (satelliteViewCamera != null) satelliteViewCamera.enabled = false;
        if (mainPlayerCamera != null) mainPlayerCamera.enabled = true;
    
        isControlling = false;
    }

    public void StartControlling()
    {
        isControlling = true;
        controlPanel.InteractionText = "";
        satelliteViewCamera.enabled = true;

        var satelliteData = satelliteViewCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        if (satelliteData != null)
        {
            satelliteData.renderType = UnityEngine.Rendering.Universal.CameraRenderType.Base;
        }
    
        mainPlayerCamera.enabled = false;

        if(PlayerControllerGun.instance != null) PlayerControllerGun.instance.canMove = false;
        QuestManager.Instance.AcceptQuest(satelliteQuest);
    }

    void Update()
    {
        if (!isControlling) return;

        HandleInput();
        UpdateQuestUI();
        CheckCompletion();
    }

    void HandleInput()
    {
        // A/D for Horizontal (Left/Right)
        float hInput = Input.GetAxis("Horizontal");
        currentH += hInput * Time.deltaTime * 20f;
        satelliteBase.localRotation = Quaternion.Euler(0, currentH, 0);

        // W/S for Vertical (Up/Down)
        float vInput = Input.GetAxis("Vertical");
        currentV += vInput * Time.deltaTime * 20f;
        currentV = Mathf.Clamp(currentV, 0, 80f); // Keep it realistic
        satelliteHead.localRotation = Quaternion.Euler(-currentV, 0, 0);
    }

    void UpdateQuestUI()
    {
    string status = $"Aligning... H: {Mathf.Round(currentH)}°/{targetHorizontal}° V: {Mathf.Round(currentV)}°/{targetVertical}° | ";
    
    QuestManager.Instance.UpdateQuestDescription(satelliteQuest.questName, status);
    }

    void CheckCompletion()
    {
        if (Mathf.Abs(currentH - targetHorizontal) < tolerance && 
            Mathf.Abs(currentV - targetVertical) < tolerance)
        {
            FinishQuest();
        }
    }

    void FinishQuest()
        {
        isControlling = false;
        satelliteViewCamera.enabled = false;
        mainPlayerCamera.enabled = true;
        QuestManager.Instance.UpdatedCompleteQuest(satelliteQuest);
        if (controlPanel != null)
        {
        controlPanel.SetPanelOffline();
        }

        QuestManager.Instance.UpdateQuestDescription(satelliteQuest.questName, "Satellite Alignment");
    
        if(PlayerControllerGun.instance != null) PlayerControllerGun.instance.canMove = true;

        QuestManager.Instance.UpdateProgress(satelliteQuest.goalItemName, 1);
    }
}