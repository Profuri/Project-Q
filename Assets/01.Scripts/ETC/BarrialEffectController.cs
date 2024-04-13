using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BarrialEffectController : MonoBehaviour
{
    private Material _material;

    private float _originThreshold;

    private readonly int _thresholdHash = Shader.PropertyToID("_Threshold");
    private readonly int _opacityHash = Shader.PropertyToID("_Opacity");

    [SerializeField] private float _appearTime;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _originThreshold = _material.GetFloat(_thresholdHash);
        _material.SetFloat(_thresholdHash, 1f);
        _material.SetFloat(_opacityHash, 0f);
    }

    public void Appear()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        
        DOTween.To(
            () => _material.GetFloat(_thresholdHash),
            threshold => _material.SetFloat(_thresholdHash, threshold),
            _originThreshold, _appearTime
        );
        DOTween.To(
            () => _material.GetFloat(_opacityHash),
            opacity => _material.SetFloat(_opacityHash, opacity),
            1f, _appearTime
        );
    }
}
