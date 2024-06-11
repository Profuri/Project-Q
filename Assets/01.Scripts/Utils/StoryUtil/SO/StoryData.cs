using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public struct StoryContent
{
    [TextArea] public string storyText;
}

[CreateAssetMenu(menuName = "SO/Story")]
public class StoryData : ScriptableObject
{
    public List<VideoClip> videoClips = new List<VideoClip>();
    public List<StoryContent> contentList = new List<StoryContent>();
}
