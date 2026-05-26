using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class SettingsManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    void Start()
    {
        if (resolutionDropdown == null)
        {
            Debug.LogWarning("SettingsManager is missing a resolution dropdown reference.", this);
            enabled = false;
            return;
        }

        resolutions = Screen.resolutions;
        if (resolutions == null || resolutions.Length == 0)
        {
            Debug.LogWarning("No screen resolutions found for SettingsManager.", this);
            enabled = false;
            return;
        }

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutions == null || resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
            return;

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
