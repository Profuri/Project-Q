using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;
using DG.Tweening;
using TMPro.Examples;
using System.Collections;
using System;

public class LaserReflectObject : InteractableObject
{
    private const float _rotateValue = 45f;
    private Coroutine _rotateSequence;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if(communicator is PlayerUnit)
        {
            if(_rotateSequence == null)
            {
                _rotateSequence = StartCoroutine(RotateSequence(1f,_rotateValue,() =>
                {
                    _rotateSequence = null;
                }));
            }
        }
        else if(communicator is LaserLauncherObject laser)
        {
            var point   = (Vector3)param[0];
            var normal  = (Vector3)param[1];
            var lastDir = ((Vector3)param[2]).normalized;
            var rayDistance = (float)param[3];    
            
            var dir = Vector3.Reflect(lastDir, normal).normalized;
            laser.SubtractDistance(rayDistance);

            laser.AddLaser(new LaserInfo { origin = point + dir.normalized, power = dir * laser.RemainDistance });
        }
    }

    private IEnumerator RotateSequence(float rotateTime,float rotateValue,Action Callback = null)
    {
        float timer = 0f;
        float percent = timer / rotateTime;
        Quaternion originRotation = transform.rotation;
        Quaternion targetRotation = originRotation * Quaternion.Euler(Vector3.up * rotateValue);
        
        while (percent < 1f)
        {
            timer += Time.deltaTime;
            percent = timer / rotateTime;

            transform.rotation = Quaternion.Lerp(originRotation, targetRotation, percent);
            yield return null;
        }
        
        UnitInfo.LocalRot = transform.localRotation;
        Callback?.Invoke();
    }
}