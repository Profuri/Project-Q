using System;
using System.Collections;
using ManagingSystem;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public enum SoundEnum
{
    EFFECT,
    BGM,
    END
}

public enum EAUDIO_MIXER
{
    MASTER,
    BGM,
    SFX
}

public class SoundManager : BaseManager<SoundManager>, IProvideSave, IProvideLoad
{
    [SerializeField] private AudioClipSO _audioClipSO;
    public AudioClipSO AudioClipSO => _audioClipSO;
    [SerializeField] private AudioClipSO _bgmClipSO;
    private AudioSource _audioSource;

    [SerializeField] private AudioMixer _masterMixer;
    [SerializeField] private AudioMixerGroup _bgmGroup;
    [SerializeField] private AudioMixerGroup _sfxGroup;
    public AudioMixerGroup SfxGroup => _sfxGroup;

    [SerializeField] private float _defaultVolume = 0f;

    public float soundFadeOnTime;

    private AudioSource[] _audioSources = new AudioSource[(int)SoundEnum.END];

    private Dictionary<EAUDIO_MIXER, float> _volumeDictionary = new Dictionary<EAUDIO_MIXER, float>();
    public Dictionary<EAUDIO_MIXER, float> VolumeDictionary
    {
        get
        {
            DataManager.Instance.LoadData(this);
            return _volumeDictionary;
        }
    }

    public override void Init()
    {
        base.Init();
        string[] soundNames = System.Enum.GetNames(typeof(SoundEnum));
        for (int i = 0; i < soundNames.Length - 1; i++)
        {
            GameObject go = new GameObject { name = soundNames[i] };
            _audioSources[i] = go.AddComponent<AudioSource>();
            _audioSources[i].playOnAwake = false;
            _audioSources[i].outputAudioMixerGroup = (soundNames[i] == "BGM" ? _bgmGroup : _sfxGroup);
            go.transform.SetParent(transform);
        }
        
        _audioSources[(int)SoundEnum.BGM].loop = true;
    }
    
    public override void StartManager()
    {
        DataManager.Instance.SettingDataProvidable(this, this);
        //DataManager.Instance.LoadData(this);
    }
    
    public void PlaySFX(string clipName,bool loop = false, SoundEffectPlayer soundEffectPlayer = null)
    {
        AudioClip clip = _audioClipSO.GetAudioClip(clipName);
        Play(clip, SoundEnum.EFFECT,loop, soundEffectPlayer);
    }

    public void PlayBGM(string clipName)
    {
        AudioClip clip = _bgmClipSO.GetAudioClip(clipName);
        Play(clip, SoundEnum.BGM);
    }

    public void Play(AudioClip audioClip, SoundEnum type = SoundEnum.EFFECT, bool loop = false, SoundEffectPlayer soundEffectPlayer = null)
    {
        if (audioClip == null)
        {
            Debug.LogError("cannot find audioclips");
            return;
        }

        if (type == SoundEnum.BGM)
        {
            StopAllCoroutines();
            AudioSource audioSource = _audioSources[(int)SoundEnum.BGM];

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.volume = 0;
            audioSource.clip = audioClip;
            audioSource.Play();

            StartCoroutine(SoundFade(true, _audioSources[(int)SoundEnum.BGM], soundFadeOnTime, 1, SoundEnum.BGM));
            StartCoroutine(SoundFade(false, _audioSources[(int)SoundEnum.BGM], soundFadeOnTime, 0, SoundEnum.BGM));
        }
        else if(type == SoundEnum.EFFECT) 
        {
            if(soundEffectPlayer == null)
            {
                AudioSource audioSource = _audioSources[(int)SoundEnum.EFFECT];
                audioSource.PlayOneShot(audioClip);
                return;
            }
            soundEffectPlayer.Play(audioClip, loop);
        }
    }

    public void Stop()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
    }
    
    public void Mute(SoundEnum type, bool mute)
    {
        _masterMixer.SetFloat(type.ToString().ToLower(), mute ? -80 : 0);
    }
    
    IEnumerator SoundFade(bool fadeIn, AudioSource source, float duration, float endVolume, SoundEnum type)
    {
        if (!fadeIn)
        {
            yield return new WaitForSeconds((float)(source.clip.length - duration));
        }

        float time = 0f;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, time / duration);
            yield return null;
        }

        if (!fadeIn)
            Play(source.clip, type);
    }


    public void SettingVolume(EAUDIO_MIXER mixerType,float volume)
    {
        float originVolume = GetOriginVolume(volume);
        _volumeDictionary[mixerType] = volume;
        _masterMixer.SetFloat(mixerType.ToString(), originVolume);
    }
    

    private float GetOriginVolume(float volume)
    {
        return Mathf.Lerp(-40, 0, volume);
    }

    public Action<SaveData> GetLoadAction()
    {
        return (saveData) =>
        {
            if (saveData.VolumeDictionary.Count < 1)
            {
                foreach (EAUDIO_MIXER eMixerType in Enum.GetValues(typeof(EAUDIO_MIXER)))
                {
                    saveData.VolumeDictionary.Add(eMixerType, _defaultVolume);
                }
                return;
            }

            Debug.Log($"MasterVolume: {saveData.VolumeDictionary[EAUDIO_MIXER.MASTER]}");

            foreach (var kvp in saveData.VolumeDictionary)
            {
                EAUDIO_MIXER eMixerType = kvp.Key;
                float volume = kvp.Value;


                if (_volumeDictionary.ContainsKey(eMixerType))
                {
                    _volumeDictionary[eMixerType] = volume;
                }
                else
                {
                    _volumeDictionary.Add(eMixerType, volume);
                    //Debug.LogError($"{eMixerType} is not exist in :{_volumeDictionary}");
                }

            }
        };
    }

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            foreach (var kvp in _volumeDictionary)
            {
                EAUDIO_MIXER eMixerType = kvp.Key;
                float volume = kvp.Value;
                saveData.VolumeDictionary[eMixerType] = volume;
            }
        };
    }
}