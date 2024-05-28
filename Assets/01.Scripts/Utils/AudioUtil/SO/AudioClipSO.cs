using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Audio/AudioClip")]
public class AudioClipSO : ScriptableObject
{
    public List<AudioClip> clipList = new List<AudioClip>();
    private Dictionary<string, AudioClip> _clipDictionary = new Dictionary<string, AudioClip>();

    public AudioClip GetAudioClip(string clipName)
    {
        foreach (AudioClip clip in clipList)
        {
            if(_clipDictionary.ContainsKey(clipName))
                return _clipDictionary[clipName];

            if (clip.name.Equals(clipName))
            {
                _clipDictionary.Add(clipName, clip);
                return clip;
            }
        }
        Debug.LogError($"Can't Find Sound: {clipName}!!!!");
        return null;
    }
}