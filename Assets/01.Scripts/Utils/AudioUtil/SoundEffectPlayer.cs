using Unity.VisualScripting;
using UnityEngine;

public class SoundEffectPlayer
{
    private readonly AudioSource _audioSource;

    public SoundEffectPlayer(Component compo)
    {
        _audioSource = compo.GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = compo.AddComponent<AudioSource>();
        }
        
        SettingAudioSource();
    }
    
    public void Play(AudioClip clip, bool loop)
    {
        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.Play();
    }

    private void SettingAudioSource()
    {
        _audioSource.outputAudioMixerGroup = SoundManager.Instance.SfxGroup;
        
        _audioSource.priority = 128;
        _audioSource.volume = 1;
        _audioSource.pitch = 1;
        _audioSource.spatialBlend = 1;

        _audioSource.dopplerLevel = 1;
        _audioSource.spread = 0;
        _audioSource.minDistance = 2;
        _audioSource.maxDistance = 3.5f;
        
        _audioSource.playOnAwake = false;
    }

    public void Stop()
    {
        _audioSource.Stop();
    }
}
