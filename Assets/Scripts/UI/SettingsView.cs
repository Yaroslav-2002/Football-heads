using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsView : View
{
    [SerializeField] Button backButton;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;


    public override void Init()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(() => ViewManager.ShowLast());
        }

        if (resolutionDropdown == null)
        {
            Debug.LogWarning("Resolution dropdown is not assigned on SettingsView.");
            return;
        }

        var availableResolutions = Screen.resolutions;

        var options = new List<string>(availableResolutions.Length);
        var addedResolutions = new HashSet<string>();
        int currentResolutionIndex = 0;
        var uniqueResolutions = new List<Resolution>();

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            var resolution = availableResolutions[i];
            string option = $"{resolution.width}x{resolution.height}";
            if (addedResolutions.Add(option))
            {
                options.Add(option);
                uniqueResolutions.Add(resolution);

                if (resolution.width == Screen.currentResolution.width &&
                    resolution.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.Count - 1;
                }
            }
        }

        resolutions = uniqueResolutions.ToArray();
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("Audio mixer is not assigned on SettingsView.");
            return;
        }

        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int optionNum)
    {
        if (resolutions == null || resolutions.Length == 0)
        {
            Debug.LogWarning("No resolutions cached when attempting to set the resolution.");
            return;
        }

        optionNum = Mathf.Clamp(optionNum, 0, resolutions.Length - 1);
        Screen.SetResolution(resolutions[optionNum].width, resolutions[optionNum].height, Screen.fullScreen);
    }
}
