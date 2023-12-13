using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ChapterData
{
    public List<Stage> Stages;
}

[CreateAssetMenu(menuName = "SO/ChapterData")]
public class ChapterDataSO : ScriptableObject
{
    public List<ChapterData> Chapters;
}