using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Playables;

public class DissolveObjectMixerBehaviour : PlayableBehaviour
{
    private List<Material> _bidingMaterials;

    private bool _isFirstFrame = true;

    private readonly int _dissolveProgressHash = Shader.PropertyToID("_DissolveProgress");
    private readonly int _visibleProgressHash = Shader.PropertyToID("_VisibleProgress");

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
        
        if (_isFirstFrame)
        {
            MaterialListSetting(playerData as GameObject);
            _isFirstFrame = false;
        }

        var weight = playable.GetInputWeight(0);

        if (weight == 0f)
        {
            _isFirstFrame = true;
        }
        else
        {
            SetMaterialValue(weight);
        }
    }

    private void MaterialListSetting(GameObject bidingObject)
    {
        var renderer = bidingObject.GetComponentsInChildren<Renderer>();
        _bidingMaterials = renderer.SelectMany(rdr => rdr.materials).ToList();
    }

    private void SetMaterialValue(float progress)
    {
        foreach (var material in _bidingMaterials)
        {
            material.SetFloat(_dissolveProgressHash, progress);
            material.SetFloat(_visibleProgressHash, progress);
        }
    }
}
