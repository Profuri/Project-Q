using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ChapterData")]
public class ChapterData : ScriptableObject
{
    public ChapterType chapter;
    public int stageCnt;

    [Header("Stage Info")] 
    public List<StageInfo> stageInfos;
}