using Unity.VisualScripting;
using UnityEngine;

public class SoundEffectPlayer
{
    private AudioSource _audioSource;

    public SoundEffectPlayer(MonoBehaviour mono)
    {
        var audioSource = mono.GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = mono.AddComponent<AudioSource>();
        }
        _audioSource = audioSource;

        SettingAudioSource();
    }
    
    public void Play(AudioClip clip, bool loop)
    {
        _audioSource.outputAudioMixerGroup ??= SoundManager.Instance.SfxGroup;
        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.Play();
    }

    private void SettingAudioSource()
    {
        _audioSource.priority = 128;
        _audioSource.volume = 1;
        _audioSource.pitch = 1;
        _audioSource.spatialBlend = 1;

        _audioSource.dopplerLevel = 1;
        _audioSource.spread = 360;
        _audioSource.minDistance = 1;
        _audioSource.maxDistance = 3;
    }

    public void Stop()
    {
        _audioSource.Stop();
    }
}
