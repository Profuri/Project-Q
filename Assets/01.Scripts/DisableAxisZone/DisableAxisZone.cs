using System.Collections;
using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;
using UnityEngine.Animations;

public class DisableAxisZone : MonoBehaviour
{
    [SerializeField] private AxisType _targetAxis;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            player.SetEnableAxis(_targetAxis, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.SetEnableAxis(_targetAxis, true);
        }
    }
}
