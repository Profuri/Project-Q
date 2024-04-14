using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class OrthoSizeChangerMixerBehaviour : PlayableBehaviour
{
    private CinemachineVirtualCamera _bidingCam;
    private float _originSize;
    
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var inputCount = playable.GetInputCount();

        if (inputCount == 0)
        {
            return;
        }

        if (_bidingCam == null)
        {
            _bidingCam = playerData as CinemachineVirtualCamera;
            if (_bidingCam == null)
            {
                return;
            }
            _originSize = _bidingCam.m_Lens.OrthographicSize;
        }

        var inputWeight = playable.GetInputWeight(0);

        var inputPlayable = (ScriptPlayable<OrthoSizeChangerBehaviour>)playable.GetInput(0);
        var input = inputPlayable.GetBehaviour();
        var targetSize = input.targetSize;

        var size = Mathf.Lerp(_originSize, targetSize, inputWeight);

        _bidingCam.m_Lens.OrthographicSize = size;
    }
}
