using System.Collections;
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
        backButton.onClick.AddListener(() => ViewManager.ShowLast());

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + "x" + resolutions[i].height);
        }
        resolutionDropdown.AddOptions(options);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int quilityInxed)
    {
        QualitySettings.SetQualityLevel(quilityInxed);
    }

    public void SetFullscreen(bool quilityInxed)
    {
        Screen.fullScreen = quilityInxed;
    }

    public void SetResolution(int optionNum)
    {
        Screen.SetResolution(resolutions[optionNum].width, resolutions[optionNum].height, Screen.fullScreen);
    }
}
