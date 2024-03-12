using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public struct TutorialStruct
{
    public VideoClip videoClip;
    public string information;
}

[CreateAssetMenu(menuName = "SO/Tutorial")]
public class TutorialSO : ScriptableObject
{
    public List<TutorialStruct> tutorialList = new List<TutorialStruct>();
}
