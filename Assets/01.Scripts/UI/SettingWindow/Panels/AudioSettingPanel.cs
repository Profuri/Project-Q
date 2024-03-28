using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioSettingPanel : WindowPanel
{
    private Dictionary<EAUDIO_MIXER, UISlider3D> _sliderDictionary;

    [SerializeField] private UISlider3D _masterSlider;
    [SerializeField] private UISlider3D _bgmSlider;
    [SerializeField] private UISlider3D _sfxSlider;

    public override void Init(SettingWindow settingWindow)
    {
        base.Init(settingWindow);

        _sliderDictionary = new Dictionary<EAUDIO_MIXER, UISlider3D>()
        {
            {EAUDIO_MIXER.MASTER,_masterSlider },{EAUDIO_MIXER.BGM,_bgmSlider },{EAUDIO_MIXER.SFX,_sfxSlider }
        };

        Debug.Log($"DictionaryCnt: {_sliderDictionary.Count}");

        foreach(KeyValuePair<EAUDIO_MIXER,UISlider3D> kvp in _sliderDictionary)
        {
            UISlider3D slider = kvp.Value;
            EAUDIO_MIXER mixerType = kvp.Key;

            Debug.Log($"Slider: {slider}");
            slider.onSliderValueChanged.AddListener((volume) => SetVolume(mixerType,volume));
        }
    }

    private void SetVolume(EAUDIO_MIXER mixerType, float volume)
    {
        SoundManager.Instance.SettingVolume(mixerType, volume);
    }

    public override void LoadPanel()
    {
        base.LoadPanel();
        LoadVolumes();
    }

    public void SaveVolumes()
    {
        DataManager.Instance.SaveData(SoundManager.Instance);
    }

    public override void ReleasePanel()
    {
        base.ReleasePanel();
        LoadVolumes();
    }

    private void LoadVolumes()
    {
        var dictionary = new Dictionary<EAUDIO_MIXER, float>(SoundManager.Instance.VolumeDictionary);
        foreach (var kvp in dictionary)
        {
            _sliderDictionary[kvp.Key].SetProgress(kvp.Value);
            SetVolume(kvp.Key, kvp.Value);
        }
    }
}