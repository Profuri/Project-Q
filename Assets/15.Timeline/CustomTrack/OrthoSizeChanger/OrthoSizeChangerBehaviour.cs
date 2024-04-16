using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class OrthoSizeChangerBehaviour : PlayableBehaviour
{
    public float originSize;
    public float targetSize;
    
    [HideInInspector] public double start;
    [HideInInspector] public double end;
    [HideInInspector] public float startWeight;
    [HideInInspector] public float endWeight;
}
