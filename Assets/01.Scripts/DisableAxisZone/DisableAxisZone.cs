using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DisableAxisZone : MonoBehaviour
{
    [SerializeField] private EAxisType _targetAxis;

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
