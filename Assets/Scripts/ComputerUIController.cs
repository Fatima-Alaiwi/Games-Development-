using UnityEngine;

public class ComputerUIController : MonoBehaviour
{
    [Header("Satellite Quest")]
    public Quest satelliteQuest; // Drag the SO asset here
    public GameObject[] satelliteSuccessImages;

    [Header("Power Cell Quest")]
    public Quest powerCellQuest; // Drag the SO asset here
    public GameObject[] powerCellSuccessImages;

    public void RefreshLogs()
    {
        // 1. Check Satellite
        if (satelliteQuest != null)
        {
            foreach (GameObject img in satelliteSuccessImages)
            {
                if (img != null) img.SetActive(satelliteQuest.isCompleted);
            }
        }

        // 2. Check Power Cell
        if (powerCellQuest != null)
        {
            foreach (GameObject img in powerCellSuccessImages)
            {
                if (img != null) img.SetActive(powerCellQuest.isCompleted);
            }
        }
    }
}