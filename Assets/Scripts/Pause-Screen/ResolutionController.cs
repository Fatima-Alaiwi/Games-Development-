using UnityEngine;
using System.Collections.Generic;

public class ResolutionController : MonoBehaviour
{
    [SerializeField] private AdvancedDropdown resolutionDropdown;
    private Resolution[] resolutions;

    void Start()
    {
        if (resolutionDropdown == null)
        {
            Debug.LogWarning("ResolutionController is missing an AdvancedDropdown reference.", this);
            enabled = false;
            return;
        }

        resolutions = Screen.resolutions;
        if (resolutions == null || resolutions.Length == 0)
        {
            Debug.LogWarning("No screen resolutions found for ResolutionController.", this);
            enabled = false;
            return;
        }
        
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

        resolutionDropdown.SelectOption(currentResolutionIndex);

        resolutionDropdown.onChangedValue += HandleResolutionChange;
    }

    private void OnDestroy()
    {
        if (resolutionDropdown != null)
            resolutionDropdown.onChangedValue -= HandleResolutionChange;
    }

    private void HandleResolutionChange(int index)
    {
        if (resolutions == null || index < 0 || index >= resolutions.Length)
            return;

        Resolution selectedRes = resolutions[index];
        Screen.SetResolution(selectedRes.width, selectedRes.height, Screen.fullScreen);
        Debug.Log($"Resolution changed to: {selectedRes.width}x{selectedRes.height}");
    }
}
