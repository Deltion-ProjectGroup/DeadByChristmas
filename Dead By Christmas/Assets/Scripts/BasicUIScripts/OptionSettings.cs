using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionSettings : MonoBehaviour {
    [Header("FullscreenOptions")]
    public GameObject fullscreenToggleSprite;
    [Header("ResolutionOptions")]
    public Dropdown resolutionDropDown;
    Resolution[] resolutions;
    public Slider mainSlider, musicSlider, soundEffectSlider;
    public AudioMixer mixer;

    public void Start()
    {
        if (fullscreenToggleSprite)
            fullscreenToggleSprite.SetActive((Screen.fullScreenMode == FullScreenMode.Windowed) ? false : true);
        resolutions = Screen.resolutions;
        resolutionDropDown.options = new List<Dropdown.OptionData>();
        List<Dropdown.OptionData> resOptions = new List<Dropdown.OptionData>();
        int index = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = resolutions[i].width.ToString() + "x" + resolutions[i].height.ToString();
            resOptions.Add(data);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                index = i;
        }
        resolutionDropDown.AddOptions(resOptions);
        resolutionDropDown.value = index;

        if (mainSlider)
        {
            float value;
            mixer.GetFloat("Master", out value);
            mainSlider.value = value;
        }
        if (musicSlider)
        {
            float value;
            mixer.GetFloat("Music", out value);
            musicSlider.value = value;
        }
        if (soundEffectSlider)
        {
            float value;
            mixer.GetFloat("SoundEffects", out value);
            soundEffectSlider.value = value;
        }
    }

    public void MainSliderChange()
    {
        mixer.SetFloat("Master", mainSlider.value);
    }
    public void MusicSliderChange()
    {
        mixer.SetFloat("Music", mainSlider.value);
    }
    public void SoundEffectSliderChange()
    {
        mixer.SetFloat("SoundEffects", mainSlider.value);
    }

    public void ToggleScreenSize()
    {
        Screen.fullScreenMode = (Screen.fullScreenMode == FullScreenMode.Windowed) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        fullscreenToggleSprite.SetActive(!fullscreenToggleSprite.activeInHierarchy);
    }

    public void OnChangeResolution()
    {
        int index = resolutionDropDown.value;
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, fullscreenToggleSprite.activeInHierarchy);
    }
}
