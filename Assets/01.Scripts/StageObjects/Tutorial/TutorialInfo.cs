using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "SO/TutorialInfo")]
public class TutorialInfo : ScriptableObject
{
    public VideoClip clip;
    public string mainText;
    [TextArea] public string descText;
}
