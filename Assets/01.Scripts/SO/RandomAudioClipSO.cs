using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class RandomClip
{
    public RandomClip(List<AudioClip> clipList)
    {
        this._clipList = clipList;
        _currentIndex = 0;
    }

    public AudioClip GetRandomClip()
    {
        if (_currentIndex >= _clipList.Count)
        {
            _currentIndex = 0;
        }
        return _clipList[_currentIndex++];
    }

    private List<AudioClip> _clipList;
    private int _currentIndex = 0;
}

[CreateAssetMenu(menuName = "SO/Audio/RandomAudioClip")]

public class RandomAudioClipSO : AudioClipSO
{
    private const int c_shuffleCnt = 100;

    public RandomClip ShuffleClip()
    {
        List<AudioClip> garbageList = clipList.ToList();

        for (int i = 0; i < c_shuffleCnt; i++)
        {
            int index = Random.Range(0, clipList.Count);
            int index2 = Random.Range(0,clipList.Count);

            if (index == index2) continue;
            garbageList.Swap(index,index2);
        }

        return new RandomClip(garbageList);
    }
}
