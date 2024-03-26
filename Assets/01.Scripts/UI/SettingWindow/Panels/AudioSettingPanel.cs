using System;
using UnityEngine;

public class AudioSettingPanel : MonoBehaviour
{
    public void SetMasterVolume(float value)
    {
        SoundManager.Instance.SettingVolume(EAUDIO_MIXER.MASTER, value);
    }
    
    public void SetBGMVolume(float value)
    {
        SoundManager.Instance.SettingVolume(EAUDIO_MIXER.BGM, value);
    }
    
    public void SetSFXVolume(float value)
    {
        SoundManager.Instance.SettingVolume(EAUDIO_MIXER.SFX, value);
    }
}