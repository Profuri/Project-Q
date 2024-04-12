using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CameraShakeMixerBehaviour : PlayableBehaviour
{
    private bool _isShaking;
    private CinemachineVirtualCamera _bindingVirtualCam;
    private CinemachineBasicMultiChannelPerlin _bindingVirtualCamPerlin;
    
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _bindingVirtualCam = playerData as CinemachineVirtualCamera;

        if (_bindingVirtualCam is null)
        {
            return;
        }

        _bindingVirtualCamPerlin = _bindingVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        var inputCount = playable.GetInputCount();
        var intensity = 0f;
        var frequency = 0f;

        var totalWeight = 0f;

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            totalWeight += inputWeight;

            if (inputWeight > 0f)
            {
                var inputPlayable = (ScriptPlayable<CameraShakeBehaviour>)playable.GetInput(0);
                var input = inputPlayable.GetBehaviour();

                intensity += input.intensity;
                frequency += input.frequency;
            }
        }

        _bindingVirtualCamPerlin.m_AmplitudeGain = intensity * totalWeight;
        _bindingVirtualCamPerlin.m_FrequencyGain = frequency * totalWeight;
    }
}
