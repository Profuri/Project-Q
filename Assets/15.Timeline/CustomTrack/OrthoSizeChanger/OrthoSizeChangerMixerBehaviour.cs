using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class OrthoSizeChangerMixerBehaviour : PlayableBehaviour
{
    private CinemachineVirtualCamera _bidingCam;
    private float _initSize;
    private bool _firstFrame;

    public override void OnGraphStart(Playable playable)
    {
        _firstFrame = true;
        base.OnGraphStart(playable);
    }
    
    public override void OnGraphStop(Playable playable)
    {
        _bidingCam.m_Lens.OrthographicSize = _initSize;
        base.OnGraphStop(playable);
    }

    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (_bidingCam == null)
        {
            _bidingCam = playerData as CinemachineVirtualCamera;
            if (_bidingCam == null)
            {
                return;
            }
        }

        if (_firstFrame)
        {
            _initSize = _bidingCam.m_Lens.OrthographicSize;
            _firstFrame = false;
        }

        var inputCount = playable.GetInputCount();

        var totalWeight = 0f;
        var originSize = 0f;
        var targetSize = 0f;

        for (var i = 0; i < inputCount; i++)
        {
            var weight = playable.GetInputWeight(i);

            if (weight > 0f)
            {
                totalWeight += weight;

                var inputPlayable = (ScriptPlayable<OrthoSizeChangerBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                originSize += input.originSize;
                targetSize += input.targetSize;
            }
        }

        if (totalWeight <= 0f)
        {
            var playableInput = (ScriptPlayable<OrthoSizeChangerBehaviour>)playable.GetInput(0);
            var input = playableInput.GetBehaviour();

            var start = input.start;
            var end = input.end;
            var startWeight = input.startWeight;
            var endWeight = input.endWeight;

            var trackTime = playable.GetTime();

            originSize = input.originSize;
            targetSize = input.targetSize;

            if (trackTime <= start)
            {
                totalWeight = startWeight;
            }
            else if (trackTime >= end)
            {
                totalWeight = endWeight;
            }
        }

        var size = Mathf.Lerp(originSize, targetSize, totalWeight);
        _bidingCam.m_Lens.OrthographicSize = size;
    }
}
