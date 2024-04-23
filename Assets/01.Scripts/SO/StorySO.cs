using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StoryContent
{
    [TextArea()] public string storyText;
}

[CreateAssetMenu(menuName = "SO/Story")]
public class StorySO : ScriptableObject
{
    public List<StoryContent> contentList = new List<StoryContent>();
}
