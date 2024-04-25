using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CinemachineOffsetChangerMixerBehaviour : PlayableBehaviour
{
    private CinemachineVirtualCamera _bindingVirtualCam;
    private CinemachineFramingTransposer _bindingFramingTransposer;

    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
        _bindingVirtualCam = null;
        _bindingFramingTransposer = null;
    }

    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (_bindingVirtualCam == null)
        {
            _bindingVirtualCam = playerData as CinemachineVirtualCamera;

            if (_bindingVirtualCam == null)
            {
                return;
            }
            
            if (_bindingFramingTransposer == null)
            {
                _bindingFramingTransposer = _bindingVirtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();

                if (_bindingFramingTransposer == null)
                {
                    return;
                }
            }
        }
        
        var inputCount = playable.GetInputCount();
        var offset = Vector3.zero;

        var totalWeight = 0f;

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            totalWeight += inputWeight;

            if (inputWeight > 0f)
            {
                var inputPlayable = (ScriptPlayable<CinemachineOffsetChangerBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                offset += input.targetOffset;
            }
        }

        _bindingFramingTransposer.m_TrackedObjectOffset = offset * totalWeight;
    }

    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
        if (_bindingFramingTransposer != null)
        {
            _bindingFramingTransposer.m_TrackedObjectOffset = Vector3.zero;
        }
    }
}
