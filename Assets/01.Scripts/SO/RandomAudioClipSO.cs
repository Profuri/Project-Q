using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "SO/Audio/RandomAudioClip")]

public class RandomAudioClipSO : AudioClipSO
{
    private const int c_shuffleCnt = 100;
    private int _currentIndex = 0;

    private List<AudioClip> _garbageClipList = new List<AudioClip>();
    
    public void ShuffleClip()
    {
        _garbageClipList = clipList.ToList();

        _currentIndex = 0;
        for (int i = 0; i < c_shuffleCnt; i++)
        {
            int index = Random.Range(0, clipList.Count);
            int index2 = Random.Range(0,clipList.Count);

            if (index == index2) continue;
            _garbageClipList.Swap(index,index2);
        }
    }

    public AudioClip GetRandomClip()
    {
        if (_currentIndex >= clipList.Count)
            ShuffleClip();
            
        return _garbageClipList[_currentIndex++];
    }
}
