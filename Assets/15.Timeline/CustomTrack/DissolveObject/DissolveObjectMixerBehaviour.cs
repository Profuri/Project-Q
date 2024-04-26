using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class DissolveObjectMixerBehaviour : PlayableBehaviour
{
    private GameObject _bidingObject;
    private List<Material> _bidingMaterials;

    private readonly int _dissolveProgressHash = Shader.PropertyToID("_DissolveProgress");
    private readonly int _visibleProgressHash = Shader.PropertyToID("_VisibleProgress");

    public override void OnGraphStart(Playable playable)
    {
        _bidingMaterials = null;
        base.OnGraphStart(playable);
    }

    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!Application.isPlaying)
        {
            return;
        }
        
        var inputCount = playable.GetInputCount();

        if (inputCount == 0)
        {
            return;
        }
        
        if (_bidingObject == null)
        {
            _bidingObject = playerData as GameObject;
            if (_bidingObject == null)
            {
                return;
            }
        }

        var weight = playable.GetInputWeight(0);

        if (weight <= 0)
        {
            var playableInput = (ScriptPlayable<DissolveObjectBehaviour>)playable.GetInput(0);
            var input = playableInput.GetBehaviour();

            var start = input.start;
            var end = input.end;
            var startWeight = input.startWeight;
            var endWeight = input.endWeight;

            var trackTime = playable.GetTime();

            if (trackTime <= start)
            {
                SetMaterialValue(startWeight);
            }
            else if (trackTime >= end)
            {
                SetMaterialValue(endWeight);    
            }
        }
        else
        {
            SetMaterialValue(weight);
        }
    }

    private void MaterialListSetting()
    {
        _bidingMaterials = new List<Material>();
        var renderer = _bidingObject.GetComponentsInChildren<Renderer>();
        _bidingMaterials = renderer.SelectMany(rdr => rdr.materials).ToList();
    }

    private void SetMaterialValue(float progress)
    {
        if (_bidingMaterials == null)
        {
            MaterialListSetting();
        }
        foreach (var material in _bidingMaterials)
        {
            material.SetFloat(_dissolveProgressHash, progress);
            material.SetFloat(_visibleProgressHash, progress);
        }
    }
}
