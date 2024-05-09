using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "SO/Audio/RandomAudioClip")]

public class RandomAudioClipSO : AudioClipSO
{
    private const int c_shuffleCnt = 100;
    private int _currentIndex = 0;
    
    public RandomAudioClipSO InstantiateClipSO()
    {
        return Instantiate(this);
    }

    private void OnEnable()
    {
        ShuffleClip();
    }

    private void ShuffleClip()
    {
        _currentIndex = 0;
        for (int i = 0; i < c_shuffleCnt; i++)
        {
            int index = Random.Range(0, clipList.Count);
            int index2 = Random.Range(0,clipList.Count);

            if (index == index2) continue;
            clipList.Swap(index,index2);
        }
    }

    public AudioClip GetRandomClip()
    {
        if (_currentIndex >= clipList.Count)
            ShuffleClip();
            
        return clipList[_currentIndex++];
    }
}
