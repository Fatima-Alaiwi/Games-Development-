using UnityEngine;

public class SatelliteController : MonoBehaviour
{
    [Header("Quest Settings")]
    public Quest satelliteQuest;
    public Camera satelliteViewCamera; // Camera showing the roof
    public Camera mainPlayerCamera;    // Your standard FPS camera
    
    [Header("Movement Parts")]
    public Transform satelliteBase;    // For Left/Right (Y-axis)
    public Transform satelliteHead;    // For Up/Down (X-axis)
    
    [Header("Target Angles")]
    public float targetHorizontal = 45f;
    public float targetVertical = 32f;
    public float tolerance = 2f; // How close they need to get

    private bool isControlling = false;
    private float currentH = 0f;
    private float currentV = 0f;

    public void StartControlling()
    {
        isControlling = true;
        mainPlayerCamera.enabled = false;
        satelliteViewCamera.enabled = true;
        
        // Lock player movement logic here
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
        // Update the quest description dynamically with current angles
        string status = $"Horizontal: {Mathf.Round(currentH)}° / {targetHorizontal}°\nVertical: {Mathf.Round(currentV)}° / {targetVertical}°";
        QuestManager.Instance.UpdateDescription(satelliteQuest.questName, status);
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
        
        if(PlayerControllerGun.instance != null) PlayerControllerGun.instance.canMove = true;

        QuestManager.Instance.UpdateProgress(satelliteQuest.goalItemName, 1);
    }
}