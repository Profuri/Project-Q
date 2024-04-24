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
            SettingAudioSource(audioSource);
        }
        _audioSource = audioSource;
        _audioSource.playOnAwake = false;
    }
    
    public void Play(AudioClip clip, bool loop)
    {
        _audioSource.outputAudioMixerGroup ??= SoundManager.Instance.SfxGroup;
        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.Play();
    }

    private void SettingAudioSource(AudioSource audioSource)
    {
        audioSource.priority = 128;
        audioSource.volume = 1;
        audioSource.pitch = 1;
        audioSource.spatialBlend = 1;

        audioSource.dopplerLevel = 1;
        audioSource.spread = 360;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 3;
    }

    public void Stop()
    {
        _audioSource.Stop();
    }
}
