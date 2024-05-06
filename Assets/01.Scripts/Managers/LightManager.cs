using System;
using System.Collections;
using AxisConvertSystem;
using ManagingSystem;
using UnityEngine;

public class LightManager : BaseManager<LightManager>
{
    [SerializeField] private Light _directionalLight;
    [SerializeField] private Light _axisDirectionalLight;

    [Header("Axis Directional Light Section")] 
    [SerializeField] private float _axisLightIntensity;
    [SerializeField] private float _axisLightTurnOnTime;
    
    public override void StartManager()
    {
        SetAxisLight(AxisType.None);
    }

    public void SetAxisLight(AxisType axis)
    {
        if (axis == AxisType.None && _axisDirectionalLight.enabled)
        {
            StartSafeCoroutine("IntensityChangeRoutine",
                IntensityChangeRoutine(_axisDirectionalLight, _axisLightIntensity, 0f, _axisLightTurnOnTime, () =>
                {
                    _axisDirectionalLight.enabled = false;
                })
            );
        }
        else if (axis != AxisType.None && !_axisDirectionalLight.enabled)
        {
            _axisDirectionalLight.enabled = true;
            StartSafeCoroutine("IntensityChangeRoutine",
                IntensityChangeRoutine(_axisDirectionalLight, 0f, _axisLightIntensity, _axisLightTurnOnTime));
        }

        var axisDirection = Vector3ExtensionMethod.GetAxisDir(axis);
        if (axisDirection.sqrMagnitude > 0)
        {
            _axisDirectionalLight.transform.rotation = Quaternion.LookRotation(-axisDirection);
        }
    }

    public void SetShadow(LightShadows shadow)
    {
        _directionalLight.shadows = shadow;
    }

    private IEnumerator IntensityChangeRoutine(Light target, float from, float to, float time, Action callBack = null)
    {
        target.intensity = from;
        var currentTime = 0f;

        while (currentTime <= time)
        {
            currentTime += Time.deltaTime;
            var percent = currentTime / time;
            var intensity = Mathf.Lerp(from, to, percent);
            target.intensity = intensity;
            yield return null;
        }

        target.intensity = to;
        callBack?.Invoke();
    }
}
