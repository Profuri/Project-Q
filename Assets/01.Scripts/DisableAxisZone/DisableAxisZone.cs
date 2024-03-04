using AxisConvertSystem;
using UnityEngine;

public class DisableAxisZone : MonoBehaviour
{
    [SerializeField] private AxisType _targetAxis;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerUnit player))
        {
            // player.SetEnableAxis(_targetAxis, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerUnit player))
        {
            // player.SetEnableAxis(_targetAxis, true);
        }
    }
}
