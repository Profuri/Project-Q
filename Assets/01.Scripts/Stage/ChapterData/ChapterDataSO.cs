using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Data/ChapterData")]
public class ChapterDataSO : ScriptableObject
{
    public List<ChapterData> data;

    public int GetStageCnt(ChapterType chapter)
    {
        return data.Find(x => x.chapter == chapter).stageCnt;
    }
}