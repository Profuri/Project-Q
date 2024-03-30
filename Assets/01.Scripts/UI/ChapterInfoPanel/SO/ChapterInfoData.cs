using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ChapterInfo
{
    public ChapterType chapter;
    public string desc;
} 

[CreateAssetMenu(menuName = "SO/Data/ChapterInfo")]
public class ChapterInfoData : ScriptableObject
{
    public List<ChapterInfo> chapterInfoList;

    public ChapterInfo GetChapterInfo(ChapterType chapter)
    {
        return chapterInfoList.FirstOrDefault(info => info.chapter == chapter);
    }
}