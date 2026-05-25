using System.Collections;
using UnityEngine;

// Attach this to any GameObject in your scene for testing save/load.
// Remove or disable it once the main menu Continue button is wired up.
public class SaveLoadTester : MonoBehaviour
{
    public enum StartMode
    {
        StartFromBeginning,
        LoadFromSave
    }

    [Tooltip("StartFromBeginning = ignore save file and play normally.\nLoadFromSave = apply the save file when the scene starts.")]
    public StartMode startMode = StartMode.LoadFromSave;

    IEnumerator Start()
    {
        if (startMode == StartMode.StartFromBeginning)
        {
            Debug.Log("[SaveLoadTester] Starting from beginning — save file ignored.");
            yield break;
        }

        if (!SaveSystem.HasSave())
        {
            Debug.Log("[SaveLoadTester] No save file found — starting from beginning.");
            yield break;
        }

        // Wait one frame so all Start() methods (HealthBar, InventorySlot) finish first.
        yield return null;

        SaveSystem.ApplyToCurrentScene();
        Debug.Log("[SaveLoadTester] Save file applied.");
    }
}
