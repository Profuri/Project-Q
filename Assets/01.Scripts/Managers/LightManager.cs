using System.Collections;
using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;

public class LightManager : BaseManager<LightManager>
{
    [SerializeField] private Light _directionalLight;
    
    public override void StartManager()
    {
        SetAxisLight(EAxisType.NONE);
    }

    public override void UpdateManager()
    {
        // Do Nothing
    }

    public void SetAxisLight(EAxisType axis)
    {
        var lightEulerAngle = new Vector3(50, -30, 0);
        
        switch (axis)
        {
            case EAxisType.X:
                lightEulerAngle = new Vector3(0, -90, 0);
                break;
            case EAxisType.Y:
                lightEulerAngle = new Vector3(90, 0, 0);
                break;
            case EAxisType.Z:
                lightEulerAngle = new Vector3(0, 0, 0);
                break;
        }

        _directionalLight.transform.eulerAngles = lightEulerAngle;
    }

    public void SetShadow(LightShadows shadow)
    {
        _directionalLight.shadows = shadow;
    }
}
