using UnityEngine;
using System.Collections.Generic;

public class ResolutionController : MonoBehaviour
{
    [SerializeField] private AdvancedDropdown resolutionDropdown;
    private Resolution[] resolutions;

    void Start()
    {
        // 1. Get all resolutions from the system
        resolutions = Screen.resolutions;
        
        // 2. Clear existing dummy options in the custom script
        resolutionDropdown.DeleteAllOptions();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string optionText = resolutions[i].width + " x " + resolutions[i].height;
            resolutionDropdown.AddOptions(optionText);

            // Identify which one the player is currently using
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // 3. Set the initial text and value
        resolutionDropdown.SelectOption(currentResolutionIndex);

        // 4. Subscribe to the Action event in your AdvancedDropdown script
        resolutionDropdown.onChangedValue += HandleResolutionChange;
    }

    private void HandleResolutionChange(int index)
    {
        Resolution selectedRes = resolutions[index];
        Screen.SetResolution(selectedRes.width, selectedRes.height, Screen.fullScreen);
        Debug.Log($"Resolution changed to: {selectedRes.width}x{selectedRes.height}");
    }
}