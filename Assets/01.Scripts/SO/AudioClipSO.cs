using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Audio/AudioClip")]
public class AudioClipSO : ScriptableObject
{
    public List<AudioClip> clipList = new List<AudioClip>();

    public AudioClip GetAudioClip(string clipName)
    {
        foreach (AudioClip clip in clipList)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        Debug.LogError($"Can't Find Sound: {clipName}!!!!");
        return null;
    }
}