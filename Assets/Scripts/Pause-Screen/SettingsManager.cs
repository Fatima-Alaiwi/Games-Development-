using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class SettingsManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    void Start()
    {
        // 1. Get all available screen resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            // Create a string for each resolution: "Width x Height"
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Check if this is the resolution the player is currently using
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // 2. Add the list to the dropdown and set the default value
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}