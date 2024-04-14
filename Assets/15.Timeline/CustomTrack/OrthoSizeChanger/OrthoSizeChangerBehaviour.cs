using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class OrthoSizeChangerBehaviour : PlayableBehaviour
{
    public float originSize;
    public float targetSize;
}
