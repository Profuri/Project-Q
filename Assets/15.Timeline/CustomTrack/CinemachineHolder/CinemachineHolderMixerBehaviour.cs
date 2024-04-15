using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CinemachineHolderMixerBehaviour : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var inputCount = playable.GetInputCount();
        var totalWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            totalWeight += inputWeight;
        }

        var bindingVirtualCam = playerData as CinemachineVirtualCamera;

        if (bindingVirtualCam == null)
        {
            return;
        }

        if (playable.GetGraph().GetRootPlayable(0).GetDuration() <= playable.GetTime())
        {
            bindingVirtualCam.m_Priority = 0;
            return;
        }

        bindingVirtualCam.m_Priority = totalWeight <= 0f ? 0 : 500;
    }
}
