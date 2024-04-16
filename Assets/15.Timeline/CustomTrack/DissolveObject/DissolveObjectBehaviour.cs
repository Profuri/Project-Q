using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class DissolveObjectBehaviour : PlayableBehaviour
{
    [HideInInspector] public double start;
    [HideInInspector] public double end;
    [HideInInspector] public float startWeight;
    [HideInInspector] public float endWeight;
}
