using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class OrthoSizeChangerMixerBehaviour : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var bidingCam = playerData as CinemachineVirtualCamera;
        
        if (bidingCam == null)
        {
            return;
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
            return;
        }

        var size = Mathf.Lerp(originSize, targetSize, totalWeight);
        bidingCam.m_Lens.OrthographicSize = size;
    }
}
