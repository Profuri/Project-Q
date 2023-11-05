using System.Collections;
using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;

public class LightManager : BaseManager<LightManager>
{
    [SerializeField] private Light _directionalLight;
    
    public override void StartManager()
    {
        // Do Nothing
    }

    public override void UpdateManager()
    {
        // Do Nothing
    }

    public void SetShadow(LightShadows shadow)
    {
        _directionalLight.shadows = shadow;
    }
}
