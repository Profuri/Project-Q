using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ChapterData")]
public class ChapterData : ScriptableObject
{
    public ChapterType chapter;
    public int stageCnt;
}