using UnityEngine;

[CreateAssetMenu(menuName = "SO/SectionData")]
public class SectionData : ScriptableObject
{
    [Header("Stage Generate Setting")] 
    public float sectionIntervalDistance;

    [Header("Bridge Setting")] 
    public float bridgeGenerateDelay;
    public float bridgeWidth;
    public float bridgeIntervalDistance;
}