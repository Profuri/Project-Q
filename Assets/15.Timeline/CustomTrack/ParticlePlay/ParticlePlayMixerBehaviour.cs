using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ParticlePlayMixerBehaviour : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var inputCount = playable.GetInputCount();

        if (inputCount == 0)
        {
            return;
        }

        var bindingParticle = playerData as ParticleSystem;

        if (bindingParticle == null)
        {
            return;
        }

        var inputWeight = playable.GetInputWeight(0);

        if (inputWeight == 0f && bindingParticle.isPlaying)
        {
            bindingParticle.Stop();
        }
        else if (inputWeight != 0f && !bindingParticle.isPlaying)
        {
            bindingParticle.Play();
        }
    }
}
