using Cinemachine;
using UnityEngine.Playables;

public class CinemachineHolderMixerBehaviour : PlayableBehaviour
{
    private CinemachineVirtualCamera _bidingCam;
    
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
        
        if (_bidingCam == null)
        {
            _bidingCam = playerData as CinemachineVirtualCamera;
            if (_bidingCam == null)
            {
                return;
            }
        }

        _bidingCam.m_Priority = totalWeight <= 0f ? 0 : 500;
    }

    public override void OnGraphStop(Playable playable)
    {
        _bidingCam.m_Priority = 0;
        base.OnGraphStop(playable);
    }
}
